using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Zametek.Utility.Logging;

namespace Company.iFX.Proxy
{
    public static class ServiceCollectionExtensions
    {
        #region Public Members

        public static IServiceCollection AddTrackingContextToOpenTelemetry(this IServiceCollection services)
        {
            services.AddProxyInterceptor(
                new AsyncTrackingContextToOpenTelemetryInterceptor().ToInterceptor);
            return services;
        }

        public static IServiceCollection IncludeErrorLogging(
            this IServiceCollection services,
            bool includeErrorLogging)
        {
            if (includeErrorLogging)
            {
                Proxy.s_DefaultLogTypes |= LogTypes.Error;
            }
            else
            {
                Proxy.s_DefaultLogTypes &= ~LogTypes.Error;
            }

            return services;
        }

        public static IServiceCollection IncludePerformanceLogging(
            this IServiceCollection services,
            bool includePerformanceLogging)
        {
            if (includePerformanceLogging)
            {
                Proxy.s_DefaultLogTypes |= LogTypes.Performance;
            }
            else
            {
                Proxy.s_DefaultLogTypes &= ~LogTypes.Performance;
            }

            return services;
        }

        public static IServiceCollection IncludeDiagnosticLogging(
            this IServiceCollection services,
            bool includeDiagnosticLogging)
        {
            if (includeDiagnosticLogging)
            {
                Proxy.s_DefaultLogTypes |= LogTypes.Diagnostic;
            }
            else
            {
                Proxy.s_DefaultLogTypes &= ~LogTypes.Diagnostic;
            }

            return services;
        }

        public static IServiceCollection IncludeInvocationLogging(
            this IServiceCollection services,
            bool includeInvocationcLogging)
        {
            if (includeInvocationcLogging)
            {
                Proxy.s_DefaultLogTypes |= LogTypes.Invocation;
            }
            else
            {
                Proxy.s_DefaultLogTypes &= ~LogTypes.Invocation;
            }

            return services;
        }

        public static IServiceCollection AddProxyInterceptor(
            this IServiceCollection services,
            Func<IInterceptor> interceptorProvider)
        {
            Proxy.s_ExtraInterceptorProviders.Add(interceptorProvider);
            return services;
        }

        #endregion
    }
}
