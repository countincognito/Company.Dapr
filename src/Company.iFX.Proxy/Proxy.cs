using Castle.DynamicProxy;
using Serilog;
using System.Diagnostics;
using Zametek.Utility.Logging;

namespace Company.iFX.Proxy
{
    public class Proxy
    {
        #region Fields

        internal static LogTypes s_DefaultLogTypes = LogTypes.Tracking | LogTypes.Error | LogTypes.Performance | LogTypes.Invocation;

        internal static IList<Func<IInterceptor>> s_ExtraInterceptorProviders = new List<Func<IInterceptor>>();

        #endregion

        #region Public Members

        public static T Create<T>(
            T instance,
            ILogger logger) where T : class
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            Debug.Assert(typeof(T) != typeof(ILogger));
            Debug.Assert(typeof(T).IsInterface);

            IList<IInterceptor> extraInterceptors = 
                s_ExtraInterceptorProviders.Select(x => x.Invoke()).ToList();

            T proxy = LogProxy.Create(
                instance,
                logger,
                s_DefaultLogTypes,
                extraInterceptors.ToArray());
            return proxy;
        }

        public static T Create<T>(
            ILogger logger) where T : class
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            Debug.Assert(typeof(T) != typeof(ILogger));
            Debug.Assert(typeof(T).IsInterface);
            return Create(Container.Container.GetService<T>(), logger);
        }

        public static T Create<T>() where T : class
        {
            Debug.Assert(typeof(T) != typeof(ILogger));
            Debug.Assert(typeof(T).IsInterface);
            return Create<T>(CreateLogger<T>());
        }

        public static ILogger CreateLogger<T>()
        {
            Debug.Assert(typeof(T).IsInterface);
            return Container.Container.GetService<ILogger>().ForContext(typeof(T));
        }

        public static object Create(
            Type instanceType,
            object instance,
            ILogger logger)
        {
            if (instanceType is null)
            {
                throw new ArgumentNullException(nameof(instanceType));
            }
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            Debug.Assert(instanceType != typeof(ILogger));
            Debug.Assert(instanceType.IsInterface);

            IList<IInterceptor> extraInterceptors =
                s_ExtraInterceptorProviders.Select(x => x.Invoke()).ToList();

            object proxy = LogProxy.Create(
                instanceType,
                instance,
                logger,
                s_DefaultLogTypes,
                extraInterceptors.ToArray());
            return proxy;
        }

        public static object Create(
            Type instanceType,
            ILogger logger)
        {
            if (instanceType is null)
            {
                throw new ArgumentNullException(nameof(instanceType));
            }
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            Debug.Assert(instanceType != typeof(ILogger));
            Debug.Assert(instanceType.IsInterface);

            return Create(
                instanceType,
                Container.Container.GetService(instanceType),
                logger);
        }

        public static object Create(Type instanceType)
        {
            if (instanceType is null)
            {
                throw new ArgumentNullException(nameof(instanceType));
            }
            Debug.Assert(instanceType != typeof(ILogger));
            Debug.Assert(instanceType.IsInterface);

            return Create(
                instanceType,
                CreateLogger(instanceType));
        }

        public static ILogger CreateLogger(Type instanceType)
        {
            Debug.Assert(instanceType.IsInterface);
            return Container.Container.GetService<ILogger>().ForContext(instanceType);
        }

        #endregion
    }
}
