using Castle.DynamicProxy;
using Company.iFX.Common;
using NATS.Client.Core;
using ProtoBuf.Grpc;
using System.Diagnostics;
using System.Reflection;
using Zametek.Utility;

namespace Company.iFX.Nats
{
    public class AsyncNatsClientInterceptor<TService>
        : AsyncInterceptorBase where TService : class
    {
        private readonly string? m_NatsUrl;

        public AsyncNatsClientInterceptor(string? natsUrl)
        {
            Type invocationTargetType = typeof(TService);
            invocationTargetType.ThrowIfNotInterface();
            m_NatsUrl = natsUrl;
        }

        protected override async Task<T> InterceptAsync<T>(
            IInvocation invocation,
            IInvocationProceedInfo proceedInfo,
            Func<IInvocation, IInvocationProceedInfo, Task<T>> proceed)
        {
            ArgumentNullException.ThrowIfNull(invocation);
            Type invocationTargetType = typeof(TService);

            object[] arguments = invocation.Arguments;

            MethodInfo methodInfo = invocation.Method;
            ParameterInfo[] parameters = methodInfo.GetParameters();
            ParameterInfo returnParameter = methodInfo.ReturnParameter;

            Debug.Assert(arguments.Length == parameters.Length);

            if (arguments.Length != Constant.NumberOfServiceMethodParameters)
            {
                throw new InvalidOperationException(
                    $@"Method '{invocation.Method.Name}' on type '{invocationTargetType.FullName}' must have {Constant.NumberOfServiceMethodParameters} parameters.");
            }

            object requestArgument = arguments[0];
            Type requestParameterType = parameters[0].ParameterType;
            object cancellationTokenArgument = arguments[1];
            Type cancellationTokenParameterType = parameters[1].ParameterType;

            // If the CancellationToken is passed in as a gRPC CallContext,
            // then convert it because NATS can only serialize standard CancellationTokens.
            if (cancellationTokenParameterType.IsAssignableTo(typeof(CallContext)))
            {
                cancellationTokenArgument = ((CallContext)cancellationTokenArgument).CancellationToken;
            }

            NatsOpts natsOpts = m_NatsUrl is null ? NatsOpts.Default : NatsOpts.Default with { Url = m_NatsUrl };

            // These will be passed to CallAsync
            var requestParameters = new object[] { requestArgument, natsOpts, null!, invocation.Method.Name, cancellationTokenArgument };

            MethodInfo method = typeof(NatsClientHelper).GetMethods().First(
                x => x.Name.Equals(nameof(NatsClientHelper.CallAsync), StringComparison.OrdinalIgnoreCase)
                && x.IsGenericMethod
                && x.GetParameters().Length == requestParameters.Length);

            MethodInfo genericMethod = method.MakeGenericMethod([typeof(TService), requestParameterType, typeof(T)]);

            return await ((Task<T>)genericMethod
                .Invoke(null, requestParameters)!)
                .ConfigureAwait(false);
        }

        protected override Task InterceptAsync(
            IInvocation invocation,
            IInvocationProceedInfo proceedInfo,
            Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            throw new InvalidOperationException();
        }
    }
}