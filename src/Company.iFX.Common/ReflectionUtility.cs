using System.Diagnostics;
using System.Reflection;

namespace Company.iFX.Common
{
    public static class ReflectionUtility
    {
        public static Func<object, TParamBase, Task<TResultBase>> CreateCovariantTaskDelegate<TParamBase, TResultBase>(MethodInfo method)
            where TParamBase : class
            where TResultBase : class
        {
            Type taskOfResultType = typeof(Task<>);
            bool returnsTaskOfResult =
                method.ReturnType.GetGenericTypeDefinition() == taskOfResultType
                && method.ReturnType.GetTypeInfo().IsConstructedGenericType;
            Debug.Assert(returnsTaskOfResult, $@"{method.Name} does not return a Task<{typeof(TResultBase)}>");

            string resultPropertyName = nameof(Task<TResultBase>.Result);
            PropertyInfo? resultProperty = method.ReturnType.GetProperty(resultPropertyName);
            Debug.Assert(resultProperty is not null);
            Debug.Assert(resultProperty.GetMethod is not null);

            Func<object, TParamBase, Task> taskFunc = CreateDelegateUnknownTargetDowncastParamUpcastResult<TParamBase, Task>(method);
            Func<Task, TResultBase> resultFunc = CreateDelegateDowncastTarget<Task, TResultBase>(resultProperty.GetMethod);

            Task<TResultBase> func(object instance, TParamBase param)
            {
                Task task = taskFunc(instance, param);
                TaskScheduler scheduler = TaskScheduler.Default;
                Task<TResultBase> resultTask = task.ContinueWith(resultFunc, scheduler);
                return resultTask;
            }

            return func;
        }

        private static Func<object, TParamBase, TResultBase> CreateDelegateUnknownTargetDowncastParamUpcastResult<TParamBase, TResultBase>(MethodInfo method)
        {
            Type reflectionUtilityType = typeof(ReflectionUtility);
            MethodInfo? genericHelper = reflectionUtilityType
                .GetMethod(nameof(CreateDelegateUnknownTargetDowncastParamUpcastResultHelper), BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert(genericHelper is not null);

            ParameterInfo[] parameters = method.GetParameters();
            Debug.Assert(parameters.Length == 1);
            Debug.Assert(parameters[0].ParameterType.IsClass);

            Type paramBaseType = typeof(TParamBase);
            Type resultBaseType = typeof(TResultBase);

            Debug.Assert(method.ReflectedType is not null);
            Debug.Assert(method.ReflectedType.IsClass || method.ReflectedType.IsInterface);
            MethodInfo constructedHelper = genericHelper
                .MakeGenericMethod(method.ReflectedType, parameters[0].ParameterType, method.ReturnType, paramBaseType, resultBaseType);

            object[] arguments = new object[] { method };
            Func<object, TParamBase, TResultBase>? func = constructedHelper.Invoke(null, arguments) as Func<object, TParamBase, TResultBase>;
            Debug.Assert(func is not null);
            return func;
        }

        private static Func<object, TParamBase, TResultBase> CreateDelegateUnknownTargetDowncastParamUpcastResultHelper<TTarget, TParam, TResult, TParamBase, TResultBase>(MethodInfo method)
            where TParam : class, TParamBase
            where TResult : TResultBase
            where TTarget : class
        {
            Func<TTarget, TParam, TResult> func = method.CreateDelegate<Func<TTarget, TParam, TResult>>();

            TResultBase resultFunc(object unknownTarget, TParamBase param)
            {
                TTarget? target = unknownTarget as TTarget;
                Debug.Assert(target is not null);

                TParam? downcastParam = param as TParam;
                Debug.Assert(downcastParam is not null);

                TResultBase upcastResult = func(target, downcastParam);
                return upcastResult;
            }

            return resultFunc;
        }

        private static Func<TTargetBase, TResult> CreateDelegateDowncastTarget<TTargetBase, TResult>(MethodInfo method)
        {
            Type reflectionUtilType = typeof(ReflectionUtility);
            MethodInfo? genericHelper = reflectionUtilType.GetMethod(nameof(CreateDelegateDowncastTargetHelper), BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert(genericHelper is not null);
            Debug.Assert(method.ReflectedType is not null);
            Debug.Assert(method.ReflectedType.IsClass);

            Type targetBaseType = typeof(TTargetBase);
            MethodInfo constructedHelper = genericHelper.MakeGenericMethod(method.ReflectedType, method.ReturnType, targetBaseType);

            object[] arguments = new object[] { method };
            Func<TTargetBase, TResult>? func = constructedHelper.Invoke(null, arguments) as Func<TTargetBase, TResult>;
            Debug.Assert(func is not null);
            return func;
        }

        private static Func<TTargetBase, TResult> CreateDelegateDowncastTargetHelper<TTarget, TResult, TTargetBase>(MethodInfo method)
            where TTarget : class, TTargetBase
        {
            Func<TTarget, TResult> func = method.CreateDelegate<Func<TTarget, TResult>>();

            TResult resultFunc(TTargetBase target)
            {
                TTarget? downcastTarget = target as TTarget;
                Debug.Assert(downcastTarget is not null);

                TResult result = func(downcastTarget);
                return result;
            }

            return resultFunc;
        }
    }
}
