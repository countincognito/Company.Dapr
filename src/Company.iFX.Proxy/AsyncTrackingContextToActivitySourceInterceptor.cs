using Castle.DynamicProxy;
using Company.iFX.Common;
using System.Diagnostics;
using System.Reflection;
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

            Type serviceType = invocation.TargetType;
            DiagnosticsConfig.NewCurrentIfEmpty(serviceType);
            string methodName = invocation.Method.Name;

            using Activity? activity = DiagnosticsConfig.Current.ActivitySource.StartActivity(methodName);

            activity?.SetTag(
                Constant.TrackingCallChainTag,
                TrackingContext.Current.CallChainId.ToDashedString());
            activity?.SetTag(
                Constant.ServiceNamespaceTag,
                serviceType.Namespace);
            activity?.SetTag(
                Constant.ServiceTypeTag,
                serviceType.Name);
            activity?.SetTag(
                Constant.ServiceMethodTag,
                methodName);

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

            Type serviceType = invocation.TargetType;
            DiagnosticsConfig.NewCurrentIfEmpty(serviceType);
            string methodName = invocation.Method.Name;

            using Activity? activity = DiagnosticsConfig.Current.ActivitySource.StartActivity(methodName);

            activity?.SetTag(
                Constant.TrackingCallChainTag,
                TrackingContext.Current.CallChainId.ToDashedString());
            activity?.SetTag(
                Constant.ServiceNamespaceTag,
                serviceType.Namespace);
            activity?.SetTag(
                Constant.ServiceTypeTag,
                serviceType.Name);
            activity?.SetTag(
                Constant.ServiceMethodTag,
                methodName);

            return await proceed(invocation, proceedInfo).ConfigureAwait(false);
        }
    }
}
