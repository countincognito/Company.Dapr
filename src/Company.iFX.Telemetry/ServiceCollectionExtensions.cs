using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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
                    .ConfigureResource(resource => resource
                        .AddService(DiagnosticsConfig.Current.ServiceName))
                    .AddAspNetCoreInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation());
        }
    }
}
