using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace Company.iFX.Configuration
{
    public static class Configuration
    {
        private static string? HostingEnvironment => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        private static readonly object s_TestLockObject = new();

        private static readonly Lazy<IConfiguration> s_Config = new(
            () =>
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
            });

        private static readonly Lazy<ConfigurationMode> s_Current = new(
            () =>
            {
                lock (s_TestLockObject)
                {
                    return new ConfigurationMode(
                        TestRequested ? ConfigurationState.UnderTest : ConfigurationState.Standard);
                }
            });

        static Configuration()
        {
            TestRequested = false;
        }

        public static bool SystemUnderTest => s_Current.Value.Mode == ConfigurationState.UnderTest;

        public static IConfiguration All => s_Config.Value;

        public static bool TestRequested
        {
            get;
            private set;
        }

        public static T? Setting<T>(string key)
        {
            IConfiguration config = s_Config.Value;
            T? value = config.GetValue<T>(key);
            return value;
        }

        //public static T? SettingOrDefault<T>(string key)
        //{

        //    return Setting<T>(key)
        //}

        public static bool PlaceUnderTest()
        {
            lock (s_TestLockObject)
            {
                TestRequested = true;
            }
            s_Current.Value.PlaceUnderTest();
            return SystemUnderTest;
        }

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
    }
}
