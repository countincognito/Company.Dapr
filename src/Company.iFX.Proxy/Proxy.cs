﻿using Castle.DynamicProxy;
using Serilog;
using System.Diagnostics;
using Zametek.Utility;
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
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentNullException.ThrowIfNull(logger);
            Debug.Assert(typeof(T) != typeof(ILogger));
            typeof(T).ThrowIfNotInterface();

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
            ArgumentNullException.ThrowIfNull(logger);
            Debug.Assert(typeof(T) != typeof(ILogger));
            typeof(T).ThrowIfNotInterface();
            return Create(Container.Container.GetService<T>(), logger);
        }

        public static T Create<T>() where T : class
        {
            Debug.Assert(typeof(T) != typeof(ILogger));
            typeof(T).ThrowIfNotInterface();
            return Create<T>(CreateLogger<T>());
        }

        public static ILogger CreateLogger<T>()
        {
            typeof(T).ThrowIfNotInterface();
            return Container.Container.GetService<ILogger>().ForContext(typeof(T));
        }

        public static object Create(
            Type instanceType,
            object instance,
            ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(instanceType);
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentNullException.ThrowIfNull(logger);
            Debug.Assert(instanceType != typeof(ILogger));
            instanceType.ThrowIfNotInterface();

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
            ArgumentNullException.ThrowIfNull(instanceType);
            ArgumentNullException.ThrowIfNull(logger);
            Debug.Assert(instanceType != typeof(ILogger));
            instanceType.ThrowIfNotInterface();

            return Create(
                instanceType,
                Container.Container.GetService(instanceType),
                logger);
        }

        public static object Create(Type instanceType)
        {
            ArgumentNullException.ThrowIfNull(instanceType);
            Debug.Assert(instanceType != typeof(ILogger));
            instanceType.ThrowIfNotInterface();

            return Create(
                instanceType,
                CreateLogger(instanceType));
        }

        public static ILogger CreateLogger(Type instanceType)
        {
            instanceType.ThrowIfNotInterface();
            return Container.Container.GetService<ILogger>().ForContext(instanceType);
        }

        #endregion
    }
}
