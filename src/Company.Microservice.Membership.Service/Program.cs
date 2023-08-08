using Asp.Versioning;
using Asp.Versioning.Builder;
using Asp.Versioning.Conventions;
using AutoMapper;
using Company.iFX.Api;
using Company.iFX.Common;
using Company.iFX.Configuration;
using Company.iFX.Dapr;
using Company.iFX.Hosting;
using Company.iFX.Logging;
using Company.iFX.Proxy;
using Company.iFX.Telemetry;
using Company.Manager.Membership.Data;
using Company.Manager.Membership.Interface;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using ProtoBuf.Grpc.Server;
using Serilog;
using System.Diagnostics;
using System.Reflection;
using Zametek.Utility.Logging;

string? ServiceName = Assembly.GetExecutingAssembly().GetName().Name;
string? BuildVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

Debug.Assert(!string.IsNullOrWhiteSpace(ServiceName));
Debug.Assert(!string.IsNullOrWhiteSpace(BuildVersion));

var apiV1_0 = new ApiVersion(1, 0);

//DiagnosticsConfig.NewCurrentIfEmpty(ServiceName);

var hostBuilder = Hosting.CreateGenericBuilder(args, @"Company")
    .ConfigureServices(services =>
    {
        services.AddScoped(_ => TrackingContextDaprClient.Create<IMembershipManager>());

        services.AddCodeFirstGrpc();
        services.AddCodeFirstGrpcReflection();

        services.AddRazorPages();
        //services.AddCors(options => {
        //    options.AddPolicy(name: "DevPolicy", policy => {
        //        policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
        //    });
        //    options.AddPolicy(name: "ProdPolicy", policy => {
        //        policy.WithOrigins("https://my.app").AllowAnyHeader().AllowAnyMethod();
        //    });
        //});
        //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //        .AddMicrosoftIdentityWebApi(Configuration.All);
        //services.AddAuthorization(options => {
        //    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        //});
        //services.AddRateLimiter(options => {
        //    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        //    RateLimitPartition.GetSlidingWindowLimiter(RateLimitUserPartitionFromClaims(httpContext),
        //            partition => new SlidingWindowRateLimiterOptions
        //            {
        //                AutoReplenishment = true,
        //                PermitLimit = 60,
        //                Window = TimeSpan.FromSeconds(60),
        //                SegmentsPerWindow = 6

        //            })
        //    );
        //    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        //});
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.UseOneOfForPolymorphism();
            options.SelectDiscriminatorNameUsing(type => Constant.DiscriminatorName);
            options.SelectDiscriminatorValueUsing(type => type.FullName);
            options.CustomSchemaIds(type => type.FullName);
        });

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolver = new PolymorphicTypeResolver();
        });

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = apiV1_0;
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            ApiVersionReader.Combine(
               //new HeaderApiVersionReader(Constant.ApiVersionString),
               new QueryStringApiVersionReader(Constant.ApiVersionString));
        });

        services.UseiFXTelemetry(ServiceName)
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder.AddConsoleExporter();

                string? zipkinHost = Configuration.Current.Setting<string>("ConnectionStrings:zipkin");
                if (!string.IsNullOrWhiteSpace(zipkinHost))
                {
                    tracerProviderBuilder.AddZipkinExporter(options => options.Endpoint = new Uri(zipkinHost));
                }
            });

        LoggerConfiguration loggerConfiguration = Logging.CreateConfiguration().WriteTo.Console();
        loggerConfiguration.Enrich.WithProperty(nameof(BuildVersion), BuildVersion);
        loggerConfiguration.Enrich.WithProperty(nameof(ServiceName), ServiceName);

        string? seqHost = Configuration.Current.Setting<string>("ConnectionStrings:seq");
        Debug.Assert(seqHost != null);
        loggerConfiguration.WriteTo.Seq(seqHost);

        Serilog.Core.Logger logger = loggerConfiguration.CreateLogger();
        Log.Logger = logger;

        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger));
        services.AddSingleton<Serilog.ILogger>(logger);

        ProxyExtensions.IncludeErrorLogging(Configuration.Current.Setting<bool>("Zametek:ErrorLogging"));
        ProxyExtensions.IncludePerformanceLogging(Configuration.Current.Setting<bool>("Zametek:PerformanceLogging"));
        ProxyExtensions.IncludeDiagnosticLogging(Configuration.Current.Setting<bool>("Zametek:DiagnosticLogging"));
        ProxyExtensions.IncludeInvocationLogging(Configuration.Current.Setting<bool>("Zametek:InvocationLogging"));
        ProxyExtensions.AddTrackingContextToOpenTelemetry();
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.Configure((ctx, app) =>
        {
            if (!ctx.HostingEnvironment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days.
                // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (ctx.HostingEnvironment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseTrackingContextMiddleware(
                (context) => new Dictionary<string, string>()
                {
                    { "Jurisdiction", "UK" },
                    { "New call-specific random string", Guid.NewGuid().ToString() }
                });

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseExceptionHandler(Handlers.ExceptionHandler);
            //app.UseCors(Configuration.Current.SettingOrDefault("CorsPolicy", "DevPolicy"));
            //app.UseAuthentication();
            //app.UseAuthorization();
            //app.UseRateLimiter();

            app.UseEndpoints(endpoints =>
            {
                ApiVersionSet versionSet = endpoints.NewApiVersionSet()
                    .HasApiVersion(apiV1_0)
                    .Build();

                endpoints.MapPost(
                    $@"{Addressing.Microservice<IMembershipManager>()}/Register",
                    async (
                        [FromServices] IMapper mapper,
                        [FromBody] Company.Microservice.Membership.Data.v1_0.RegisterRequestDtoBase registerRequestDto) =>
                    {
                        RegisterRequestBase registerRequest = mapper.Map<RegisterRequestBase>(registerRequestDto);

                        IMembershipManager membershipManager = Proxy.Create<IMembershipManager>();
                        RegisterResponseBase response = await membershipManager.RegisterMemberAsync(registerRequest);

                        Company.Microservice.Membership.Data.v1_0.RegisterResponseDtoBase registerResponse =
                            mapper.Map<Company.Microservice.Membership.Data.v1_0.RegisterResponseDtoBase>(response);

                        return Results.Ok(registerResponse);
                    })
                    .WithApiVersionSet(versionSet)
                    .HasApiVersion(apiV1_0)
                    .Produces<Company.Microservice.Membership.Data.v1_0.RegisterResponseDtoBase>();
            });
        });
    });

await hostBuilder.RunAsync();
