using Zametek.Utility;

namespace Company.iFX.Test
{
    public static class ServiceRunner
    {
        public static Func<T, Task> Create<T>(Func<T, Task> serviceRunner) where T : class
        {
            return serviceRunner;
        }

        public static string GenerateRandomString()
        {
            return Guid.NewGuid().ToFlatString();
        }
    }
}
