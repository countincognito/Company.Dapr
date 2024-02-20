using System.Diagnostics;

namespace Company.iFX.Common
{
    public class DiagnosticsConfig
    {
        private readonly string m_ServiceName;
        private readonly ActivitySource m_ActivitySource;

        public DiagnosticsConfig(string serviceName)
        {
            m_ServiceName = serviceName;
            m_ActivitySource = new ActivitySource(serviceName);
        }

        public string ServiceName => m_ServiceName;

        public ActivitySource ActivitySource => m_ActivitySource;

        #region Static

        private static readonly object s_LockObject = new();
        private static string? s_ServiceName;

        private static readonly Lazy<DiagnosticsConfig> s_Current = new(
            () =>
            {
                lock (s_LockObject)
                {
                    if (string.IsNullOrWhiteSpace(s_ServiceName))
                    {
                        throw new InvalidOperationException("Service name must be set before accessing diagnostic configuration");
                    }

                    return new DiagnosticsConfig(s_ServiceName);
                }
            });

        public static DiagnosticsConfig Current => s_Current.Value;

        public static DiagnosticsConfig NewCurrent(string serviceName)
        {
            lock (s_LockObject)
            {
                if (s_Current.IsValueCreated)
                {
                    throw new InvalidOperationException("Service name has already been set for diagnostic configuration");
                }

                s_ServiceName = serviceName;
                return s_Current.Value;
            }
        }

        public static void NewCurrentIfEmpty<T>()
        {
            lock (s_LockObject)
            {
                NewCurrentIfEmpty(typeof(T));
            }
        }

        public static void NewCurrentIfEmpty(Type serviceType)
        {
            lock (s_LockObject)
            {
                string serviceName = serviceType.FullName ??
                    throw new ArgumentNullException($@"Unable to get fullname for Type {serviceType.Name}.");
                NewCurrentIfEmpty(serviceName);
            }
        }

        public static void NewCurrentIfEmpty(string serviceName)
        {
            lock (s_LockObject)
            {
                if (!s_Current.IsValueCreated)
                {
                    NewCurrent(serviceName);
                }
            }
        }

        #endregion
    }
}
