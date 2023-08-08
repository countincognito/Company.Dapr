using Castle.DynamicProxy;
using Zametek.Utility.Logging;

namespace Company.iFX.Proxy
{
    public static class ProxyExtensions
    {
        #region Public Members

        public static void AddTrackingContextToActivitySource()
        {
            AddInterceptor(new AsyncTrackingContextToActivitySourceInterceptor().ToInterceptor);
        }

        public static void IncludeErrorLogging(bool includeErrorLogging)
        {
            if (includeErrorLogging)
            {
                Proxy.s_DefaultLogTypes |= LogTypes.Error;
            }
            else
            {
                Proxy.s_DefaultLogTypes &= ~LogTypes.Error;
            }
        }

        public static void IncludePerformanceLogging(bool includePerformanceLogging)
        {
            if (includePerformanceLogging)
            {
                Proxy.s_DefaultLogTypes |= LogTypes.Performance;
            }
            else
            {
                Proxy.s_DefaultLogTypes &= ~LogTypes.Performance;
            }
        }

        public static void IncludeDiagnosticLogging(bool includeDiagnosticLogging)
        {
            if (includeDiagnosticLogging)
            {
                Proxy.s_DefaultLogTypes |= LogTypes.Diagnostic;
            }
            else
            {
                Proxy.s_DefaultLogTypes &= ~LogTypes.Diagnostic;
            }
        }

        public static void IncludeInvocationLogging(bool includeInvocationcLogging)
        {
            if (includeInvocationcLogging)
            {
                Proxy.s_DefaultLogTypes |= LogTypes.Invocation;
            }
            else
            {
                Proxy.s_DefaultLogTypes &= ~LogTypes.Invocation;
            }
        }

        public static void AddInterceptor(Func<IInterceptor> interceptorProvider)
        {
            Proxy.s_ExtraInterceptorProviders.Add(interceptorProvider);
        }

        #endregion
    }
}
