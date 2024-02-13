using Castle.DynamicProxy;
using Company.iFX.Common;
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
            ArgumentNullException.ThrowIfNull(invocation);
            ArgumentNullException.ThrowIfNull(proceedInfo);
            ArgumentNullException.ThrowIfNull(proceed);

            TrackingContext.NewCurrentIfEmpty();
            using Activity? activity = DiagnosticsConfig.Current.ActivitySource.StartActivity(invocation.Method.Name);

            activity?.SetTag(
                Constant.TrackingCallChainTag,
                TrackingContext.Current.CallChainId.ToDashedString());
            activity?.SetTag(
                Constant.ServiceNamespaceTag,
                invocation.TargetType?.Namespace);
            activity?.SetTag(
                Constant.ServiceTypeTag,
                invocation.TargetType?.Name);
            activity?.SetTag(
                Constant.ServiceMethodTag,
                invocation.Method?.Name);

            await proceed(invocation, proceedInfo).ConfigureAwait(false);
        }

        protected override async Task<T> InterceptAsync<T>(
            IInvocation invocation,
            IInvocationProceedInfo proceedInfo,
            Func<IInvocation, IInvocationProceedInfo, Task<T>> proceed)
        {
            ArgumentNullException.ThrowIfNull(invocation);
            ArgumentNullException.ThrowIfNull(proceedInfo);
            ArgumentNullException.ThrowIfNull(proceed);

            TrackingContext.NewCurrentIfEmpty();
            using Activity? activity = DiagnosticsConfig.Current.ActivitySource.StartActivity(invocation.Method.Name);

            activity?.SetTag(
                Constant.TrackingCallChainTag,
                TrackingContext.Current.CallChainId.ToDashedString());
            activity?.SetTag(
                Constant.ServiceNamespaceTag,
                invocation.TargetType?.Namespace);
            activity?.SetTag(
                Constant.ServiceTypeTag,
                invocation.TargetType?.Name);
            activity?.SetTag(
                Constant.ServiceMethodTag,
                invocation.Method?.Name);

            return await proceed(invocation, proceedInfo).ConfigureAwait(false);
        }
    }
}
