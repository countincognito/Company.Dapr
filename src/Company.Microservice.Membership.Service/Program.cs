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
using Company.Manager.Membership.Data;
using Company.Manager.Membership.Interface;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProtoBuf.Grpc.Server;
using Serilog;
using System.Reflection;
using Zametek.Utility.Logging;

string? ServiceName = Assembly.GetExecutingAssembly().GetName().Name;
string? BuildVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

var apiV1_0 = new ApiVersion(1, 0);

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
            options.SelectDiscriminatorNameUsing(type => "$type");
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
               //new HeaderApiVersionReader("Api-Version"),
               new QueryStringApiVersionReader("Api-Version"));
        });

        LoggerConfiguration loggerConfiguration = Logging.CreateConfiguration().WriteTo.Console();

        if (BuildVersion is not null)
        {
            loggerConfiguration.Enrich.WithProperty(nameof(BuildVersion), BuildVersion);
        }
        if (ServiceName is not null)
        {
            loggerConfiguration.Enrich.WithProperty(nameof(ServiceName), ServiceName);
        }

        if (Configuration.IsDevelopment())
        {
            loggerConfiguration.WriteTo.Seq("http://localhost:5341");
        }

        Serilog.Core.Logger logger = loggerConfiguration.CreateLogger();
        Log.Logger = logger;

        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger));
        services.AddSingleton<Serilog.ILogger>(logger);

        services.IncludeErrorLogging(Configuration.Setting<bool>("Zametek:ErrorLogging"));
        services.IncludePerformanceLogging(Configuration.Setting<bool>("Zametek:PerformanceLogging"));
        services.IncludeDiagnosticLogging(Configuration.Setting<bool>("Zametek:DiagnosticLogging"));
        services.IncludeInvocationLogging(Configuration.Setting<bool>("Zametek:InvocationLogging"));
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

            app.UseExceptionHandler(ExceptionHandler);
            //app.UseCors(Configuration.SettingOrDefault("CorsPolicy", "DevPolicy"));
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

static void ExceptionHandler(IApplicationBuilder applicationBuilder)
{
    applicationBuilder.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        IExceptionHandlerPathFeature? exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        Exception? exception = exceptionHandlerPathFeature?.Error;
        if (exception?.InnerException is AggregateException
            && exception.InnerException?.InnerException is HttpRequestException)
        {
            await context.Response.WriteAsync("Network or server error calling down stream service");
        };
    });
}
