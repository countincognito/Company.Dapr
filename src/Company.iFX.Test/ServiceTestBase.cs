using System.Diagnostics;

namespace Company.iFX.Test
{
    public abstract class ServiceTestBase
    {
        Type[] m_ServicesUnderTest = Array.Empty<Type>();

        public void Setup(params Type[] types)
        {
            Configuration.Configuration.ActivateTest();

            if (!Configuration.Configuration.SystemUnderTest)
            {
                throw new InvalidOperationException("Configuration must be in test mode to run UnitTests");
            }

            m_ServicesUnderTest = types;
            Container.Container.ConfigureTesting(
                Logging.Logging.CreateConfiguration().CreateLogger(),
                types);
        }

        public void Cleanup()
        {
            m_ServicesUnderTest = Array.Empty<Type>();
        }

        public Task TestService<T>(
            Func<T, Task> serviceRunner,
            params object[] mocks) where T : class
        {
            return MockServiceEnvironment(GetServiceType<T>(), serviceRunner, mocks);
        }

        private Type GetServiceType<T>()
        {
            return m_ServicesUnderTest.Single(t => t.GetInterfaces().Contains(typeof(T)));
        }

        private static async Task MockServiceEnvironment<T>(
            Type targetType,
            Func<T, Task> serviceRunner,
            params object[] mocks) where T : class
        {
            Container.Container.CreateTestScope(mocks);
            try
            {
                T? instance = default;
                instance = Activator.CreateInstance(targetType) as T;
                Debug.Assert(instance is not null);
                await serviceRunner.Invoke(instance);
            }
            finally
            {
                Container.Container.EndScope();
            }
        }
    }
}
