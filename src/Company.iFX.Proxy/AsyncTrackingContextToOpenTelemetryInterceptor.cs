using Castle.DynamicProxy;
using System.Diagnostics;
using Zametek.Utility;

namespace Company.iFX.Proxy
{
    public class AsyncTrackingContextToOpenTelemetryInterceptor
        : AsyncInterceptorBase
    {
        protected override async Task InterceptAsync(
            IInvocation invocation,
            IInvocationProceedInfo proceedInfo,
            Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            if (invocation is null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            if (proceedInfo is null)
            {
                throw new ArgumentNullException(nameof(proceedInfo));
            }
            if (proceed is null)
            {
                throw new ArgumentNullException(nameof(proceed));
            }

            AddCallChainIdToActivity();
            await proceed(invocation, proceedInfo);
        }

        protected override async Task<T> InterceptAsync<T>(
            IInvocation invocation,
            IInvocationProceedInfo proceedInfo,
            Func<IInvocation, IInvocationProceedInfo, Task<T>> proceed)
        {
            if (invocation is null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            if (proceedInfo is null)
            {
                throw new ArgumentNullException(nameof(proceedInfo));
            }
            if (proceed is null)
            {
                throw new ArgumentNullException(nameof(proceed));
            }

            AddCallChainIdToActivity();
            return await proceed(invocation, proceedInfo);
        }

        private static void AddCallChainIdToActivity()
        {
            TrackingContext.NewCurrentIfEmpty();
            var activity = Activity.Current;
            activity?.SetTag(
                nameof(TrackingContext.CallChainId),
                TrackingContext.Current.CallChainId.ToDashedString());
        }
    }
}
