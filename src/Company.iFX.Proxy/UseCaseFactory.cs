using Company.iFX.Common;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Zametek.Utility.Logging;

namespace Company.iFX.Proxy
{
    public static class UseCaseFactory<T, C, R>
        where T : class
        where C : class
        where R : class
    {
        private delegate Task<R> UseCase(C criteria);

        public static Task<R> CallAsync(
            C criteria,
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
            Debug.Assert(useCaseInterfaceType is not null, "You did not follow the rules...");
            useCaseInterfaceType.ThrowIfNotInterface();

            MethodInfo? methodName = useCaseInterfaceType.GetMethod(callerName);
            Debug.Assert(methodName is not null, $@"{callerName} not found");

            Func<object, C, Task<R>> useCaseFunc = ReflectionUtility.CreateCovariantTaskDelegate<C, R>(methodName);

            Task<R> useCase(C criteria)
            {
                object instance = Proxy.Create(useCaseInterfaceType);
                Debug.Assert(instance is not null);

                Task<R> func()
                {
                    Task<R> task = useCaseFunc(instance, criteria);
                    return task;
                }

                Task<R> task = Task.Run(func);
                return task;
            }

            return useCase(criteria);
        }
    }
}
