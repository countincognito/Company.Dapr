using Company.Access.User.Impl;
using Company.Access.User.Service;
using Company.iFX.Configuration;
using Company.iFX.Dapr;
using Company.iFX.Grpc;
using Company.iFX.Hosting;
using Company.iFX.Logging;
using Company.iFX.Proxy;
using Company.iFX.Telemetry;
using Company.Utility.Cache.Interface;
using Company.Utility.Encryption.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Polly;
using ProtoBuf.Grpc.Server;
using ProtoBuf.Meta;
using Serilog;
using System.Diagnostics;
using System.Reflection;

string? ServiceName = Assembly.GetExecutingAssembly().GetName().Name;
string? BuildVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

Debug.Assert(!string.IsNullOrWhiteSpace(ServiceName));
Debug.Assert(!string.IsNullOrWhiteSpace(BuildVersion));

//DiagnosticsConfig.NewCurrentIfEmpty(ServiceName);

var hostBuilder = Hosting.CreateGenericBuilder(args, @"Company", @"Zametek")
    .ConfigureServices(services =>
    {
        // Protobuf-net cannot serialize DateTimeOffset
        RuntimeTypeModel.Default.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(DateTimeOffsetSurrogate));
        services.AddScoped(_ => TrackingContextDaprClient.Create<IEncryptionUtility>());
        services.AddScoped(_ => TrackingContextDaprClient.Create<ICacheUtility>());
        services.AddTrackingContextGrpcInterceptor();

        services.AddCodeFirstGrpc();
        services.AddCodeFirstGrpcReflection();

        string? otelHost = Configuration.Current.Setting<string>("ConnectionStrings:otel");

        services.UseiFXTelemetry(ServiceName)
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder.AddConsoleExporter();

                if (!string.IsNullOrWhiteSpace(otelHost))
                {
                    tracerProviderBuilder.AddOtlpExporter(options => options.Endpoint = new Uri(otelHost));
                }
            })
            .WithMetrics(meterProviderBuilder =>
            {
                meterProviderBuilder.AddConsoleExporter();

                if (!string.IsNullOrWhiteSpace(otelHost))
                {
                    meterProviderBuilder.AddOtlpExporter(options => options.Endpoint = new Uri(otelHost));
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

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddSerilog(logger);
            loggingBuilder.AddOpenTelemetry(otelBuilder =>
            {
                otelBuilder.IncludeFormattedMessage = true;
                otelBuilder.IncludeScopes = true;
                otelBuilder.ParseStateValues = true;

                otelBuilder.AddConsoleExporter();

                if (!string.IsNullOrWhiteSpace(otelHost))
                {
                    otelBuilder.AddOtlpExporter(options => options.Endpoint = new Uri(otelHost));
                }
            });
        });
        services.AddSingleton<Serilog.ILogger>(logger);

        ProxyExtensions.IncludeErrorLogging(Configuration.Current.Setting<bool>("Zametek:ErrorLogging"));
        ProxyExtensions.IncludePerformanceLogging(Configuration.Current.Setting<bool>("Zametek:PerformanceLogging"));
        ProxyExtensions.IncludeDiagnosticLogging(Configuration.Current.Setting<bool>("Zametek:DiagnosticLogging"));
        ProxyExtensions.IncludeInvocationLogging(Configuration.Current.Setting<bool>("Zametek:InvocationLogging"));
        ProxyExtensions.AddTrackingContextToActivitySource();

        services.AddPooledDbContextFactory<UserContext>(
            options => options.UseNpgsql(Configuration.Current.Setting<string>("ConnectionStrings:postgres_users")));
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.Configure((ctx, app) =>
        {
            var migrateDbPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

            migrateDbPolicy.Execute(async () =>
            {
                IDbContextFactory<UserContext> userCtxFactory = app.ApplicationServices.GetRequiredService<IDbContextFactory<UserContext>>();
                using UserContext userCtx = await userCtxFactory
                    .CreateDbContextAsync()
                    .ConfigureAwait(false);
                DatabaseFacade userDb = userCtx.Database;
                await userDb.MigrateAsync().ConfigureAwait(false);
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<UserAccessProxy>();
                endpoints.MapCodeFirstGrpcReflectionService();

                endpoints.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            });
        });
    });

await hostBuilder.RunAsync().ConfigureAwait(false);
