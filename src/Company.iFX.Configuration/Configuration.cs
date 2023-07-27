using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Company.iFX.Configuration
{
    public class Configuration
    {
        private readonly bool m_TestRequested = false;
        private readonly IConfiguration m_Config;
        private readonly ConfigurationMode m_ConfigMode;

        public Configuration(bool testRequested)
        {
            m_TestRequested = testRequested;
            m_Config = LoadConfiguration();
            m_ConfigMode = SetConfigurationMode(m_TestRequested);
        }

        public IConfiguration All => m_Config;

        public ConfigurationMode Mode => m_ConfigMode;

        public T? Setting<T>(string key)
        {
            IConfiguration config = m_Config;
            T? value = config.GetValue<T>(key);
            return value;
        }

        public T? SettingOrDefault<T>(string key)
        {
            return Setting<T>(key) ?? default;
        }

        #region Static

        private static readonly object s_LockObject = new();
        private static bool s_TestRequested = false;

        private static readonly Lazy<Configuration> s_Current = new(
            () =>
            {
                lock (s_LockObject)
                {
                    return new Configuration(s_TestRequested);
                }
            });

        public static Configuration Current => s_Current.Value;

        public static bool PlaceUnderTest()
        {
            lock (s_LockObject)
            {
                s_TestRequested = true;
                return SystemUnderTest;
            }
        }
        public static bool SystemUnderTest => Current.Mode.State == ConfigurationState.Test;

        public static bool IsDevelopment()
        {
            return IsEnvironment(Environments.Development);
        }

        public static bool IsStaging()
        {
            return IsEnvironment(Environments.Staging);
        }

        public static bool IsProduction()
        {
            return IsEnvironment(Environments.Production);
        }

        public static bool IsEnvironment(string environmentName)
        {
            return string.Equals(
                HostingEnvironment,
                environmentName,
                StringComparison.OrdinalIgnoreCase);
        }

        private static string? HostingEnvironment => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        private static IConfiguration LoadConfiguration()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder.AddJsonFile(@"appsettings.json", optional: true, reloadOnChange: true);

            string? hostingEnvironment = HostingEnvironment;

            if (hostingEnvironment is not null)
            {
                configBuilder.AddJsonFile($@"appsettings.{hostingEnvironment}.json", optional: true, reloadOnChange: true);
            }

            configBuilder.AddEnvironmentVariables();
            return configBuilder.Build();
        }
        private static ConfigurationMode SetConfigurationMode(bool testRequested)
        {
            return new ConfigurationMode(testRequested ? ConfigurationState.Test : ConfigurationState.Standard);
        }

        #endregion
    }
}
