using Castle.DynamicProxy;
using ProtoBuf.Grpc;
using System.Diagnostics;
using System.Reflection;
using Zametek.Utility;

namespace Company.iFX.Nats
{
    public class AsyncNatsClientInterceptor<TService>
        : AsyncInterceptorBase where TService : class
    {
        private const int c_MaxRequestParameters = 2;
        private const int c_CallAsyncParameters = 5;

        public AsyncNatsClientInterceptor()
        {
            Type invocationTargetType = typeof(TService);
            invocationTargetType.ThrowIfNotInterface();
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

            if (arguments.Length != c_MaxRequestParameters)
            {
                throw new InvalidOperationException(
                    $@"Method '{invocation.Method.Name}' on type '{invocationTargetType.FullName}' must have {c_MaxRequestParameters} parameters.");
            }

            object requestArgument = arguments[0];
            Type requestParameterType = parameters[0].ParameterType;
            object cancellationTokenArgument = arguments[1];
            Type cancellationTokenParameterType = parameters[1].ParameterType;

            if (cancellationTokenParameterType.IsAssignableTo(typeof(CallContext)))
            {
                cancellationTokenArgument = ((CallContext)cancellationTokenArgument).CancellationToken;
            }

            MethodInfo method = typeof(NatsClientHelper).GetMethods().First(
                x => x.Name.Equals(nameof(NatsClientHelper.CallAsync), StringComparison.OrdinalIgnoreCase)
                && x.IsGenericMethod
                && x.GetParameters().Length == c_CallAsyncParameters);

            MethodInfo genericMethod = method.MakeGenericMethod([typeof(TService), requestParameterType, typeof(T)]);

            return await ((Task<T>)genericMethod
                .Invoke(null, new object[] { requestArgument, null!, null!, invocation.Method.Name, cancellationTokenArgument })!)
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