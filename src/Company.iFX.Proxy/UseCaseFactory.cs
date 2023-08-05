using Company.iFX.Common;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Zametek.Utility;

namespace Company.iFX.Proxy
{
    public static class UseCaseFactory<T, C1, C2, R>
        where T : class
        where C1 : class
        where C2 : struct
        where R : class
    {
        private delegate Task<R> UseCase(C1 criteria, C2 context);

        public static Task<R> CallAsync(
            C1 criteria,
            C2 context,
            [CallerMemberName] string? callerName = null)
        {
            // DESIGN NOTE: Do not await in the strategy. Let the caller await.
            // Refer to Service Granularity Module for general rationale as to why this technique is more than fast enough.
            // 'Invoked' method operates on the same thread up to the first await, then returns control to the caller.
            // To defer method initialization to another thread, wrap in Task.Run(). See Stephen Toub et al.

            // DESIGN NOTE: Default convention.
            // Evolve your factories to allow consumers to inject a 'contextual' convention.

            typeof(T).ThrowIfNotInterface();
            Debug.Assert(callerName is not null);

            string? criteriaNamespace = criteria.GetType().Namespace;
            Debug.Assert(criteriaNamespace is not null);

            string useCaseTypeName = $"{criteriaNamespace.Replace(ConventionKeyword.Data, ConventionKeyword.Interface)}.I{ComponentKeyword.UseCases}";

            Type? useCaseInterfaceType = Assembly.GetAssembly(typeof(T))?.GetType(useCaseTypeName);
            Debug.Assert(useCaseInterfaceType is not null, $@"Could not find the type {useCaseInterfaceType}");
            useCaseInterfaceType.ThrowIfNotInterface();

            MethodInfo? methodName = useCaseInterfaceType.GetMethod(callerName);
            Debug.Assert(methodName is not null, $@"{callerName} not found in the type {useCaseInterfaceType}");

            Func<object, C1, C2, Task<R>> useCaseFunc = ReflectionUtility.CreateCovariantTaskDelegate<C1, C2, R>(methodName);

            Task<R> useCase(C1 criteria, C2 context)
            {
                object instance = Proxy.Create(useCaseInterfaceType);
                Debug.Assert(instance is not null);

                Task<R> func()
                {
                    Task<R> task = useCaseFunc(instance, criteria, context);
                    return task;
                }

                Task<R> task = Task.Run(func);
                return task;
            }

            // Do not await here. Allow the caller to await.
            return useCase(criteria, context);
        }
    }
}
