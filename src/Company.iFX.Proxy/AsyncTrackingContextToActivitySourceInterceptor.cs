using Castle.DynamicProxy;
using Company.iFX.Telemetry;
using System.Diagnostics;
using Zametek.Utility;

namespace Company.iFX.Proxy
{
    public class AsyncTrackingContextToActivitySourceInterceptor
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

            TrackingContext.NewCurrentIfEmpty();
            using Activity? activity = DiagnosticsConfig.Current.ActivitySource.StartActivity(invocation.Method.Name);

            activity?.SetTag(
                nameof(TrackingContext.CallChainId),
                TrackingContext.Current.CallChainId.ToDashedString());
            activity?.SetTag(
                nameof(invocation.TargetType.Namespace),
                invocation.TargetType?.Namespace);
            activity?.SetTag(
                nameof(invocation.TargetType),
                invocation.TargetType?.Name);
            activity?.SetTag(
                nameof(invocation.Method),
                invocation.Method?.Name);

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

            TrackingContext.NewCurrentIfEmpty();
            using Activity? activity = DiagnosticsConfig.Current.ActivitySource.StartActivity(invocation.Method.Name);

            activity?.SetTag(
                nameof(TrackingContext.CallChainId),
                TrackingContext.Current.CallChainId.ToDashedString());
            activity?.SetTag(
                nameof(invocation.TargetType.Namespace),
                invocation.TargetType?.Namespace);
            activity?.SetTag(
                nameof(invocation.TargetType),
                invocation.TargetType?.Name);
            activity?.SetTag(
                nameof(invocation.Method),
                invocation.Method?.Name);

            return await proceed(invocation, proceedInfo);
        }
    }
}
