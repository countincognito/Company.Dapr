using Company.iFX.Common;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Zametek.Utility;

namespace Company.iFX.Proxy
{
    public static class UseCaseFactory<TService, TParamBase, TContext, TResultBase>
        where TService : class
        where TParamBase : class
        where TContext : struct
        where TResultBase : class
    {
        public static Task<TResultBase> CallAsync(
            TParamBase paramBase,
            TContext context,
            [CallerMemberName] string? callerName = null)
        {
            // DESIGN NOTE: Do not await in the strategy. Let the caller await.
            // Refer to Service Granularity Module for general rationale as to why this technique is more than fast enough.
            // 'Invoked' method operates on the same thread up to the first await, then returns control to the caller.
            // To defer method initialization to another thread, wrap in Task.Run(). See Stephen Toub et al.

            // DESIGN NOTE: Default convention.
            // Evolve your factories to allow consumers to inject a 'contextual' convention.

            typeof(TService).ThrowIfNotInterface();
            Debug.Assert(callerName is not null);

            string? criteriaNamespace = paramBase.GetType().Namespace;
            Debug.Assert(criteriaNamespace is not null);

            string useCaseTypeName = $"{criteriaNamespace.Replace(ConventionKeyword.Data, ConventionKeyword.Interface)}.I{ComponentKeyword.UseCases}";

            Type? useCaseInterfaceType = Assembly.GetAssembly(typeof(TService))?.GetType(useCaseTypeName);
            Debug.Assert(useCaseInterfaceType is not null, $@"Could not find the type {useCaseInterfaceType}");
            useCaseInterfaceType.ThrowIfNotInterface();

            MethodInfo? methodName = useCaseInterfaceType.GetMethod(callerName);
            Debug.Assert(methodName is not null, $@"{callerName} not found in the type {useCaseInterfaceType}");

            Func<object, TParamBase, TContext, Task<TResultBase>> useCaseFunc = ReflectionUtility.CreateCovariantTaskDelegate<TParamBase, TContext, TResultBase>(methodName);

            Task<TResultBase> ConvertUseCaseMethod(TParamBase paramBase, TContext context)
            {
                object instance = Proxy.Create(useCaseInterfaceType);
                Debug.Assert(instance is not null);

                Task<TResultBase> func()
                {
                    Task<TResultBase> task = useCaseFunc(instance, paramBase, context);
                    return task;
                }

                Task<TResultBase> task = Task.Run(func);
                return task;
            }

            // Do not await here. Let the caller do the awaiting.
            return ConvertUseCaseMethod(paramBase, context);
        }
    }
}
