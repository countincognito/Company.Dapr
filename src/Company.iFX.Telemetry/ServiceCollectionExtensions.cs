using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Company.iFX.Common;

namespace Company.iFX.Telemetry
{
    public static class ServiceCollectionExtensions
    {
        public static OpenTelemetryBuilder UseiFXTelemetry(
            this IServiceCollection services,
            string serviceName)
        {
            DiagnosticsConfig.NewCurrentIfEmpty(serviceName);

            return services.AddOpenTelemetry()
                .WithTracing(tracerProviderBuilder => tracerProviderBuilder
                    .AddSource(DiagnosticsConfig.Current.ActivitySource.Name)
                    .ConfigureResource(resourceBuilder => resourceBuilder
                        .AddService(DiagnosticsConfig.Current.ServiceName))
                    .AddAspNetCoreInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation())
                .WithMetrics(meterProviderBuilder => meterProviderBuilder
                    .ConfigureResource(resourceBuilder => resourceBuilder
                        .AddService(DiagnosticsConfig.Current.ServiceName))
                    //.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(DiagnosticsConfig.Current.ServiceName))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddProcessInstrumentation()
                    .AddRuntimeInstrumentation());
        }
    }
}
