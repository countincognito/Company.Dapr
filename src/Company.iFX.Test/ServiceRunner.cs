using Zametek.Utility;

namespace Company.iFX.Test
{
    public static class ServiceRunner
    {
        private static readonly DateTime s_StartDate = new(1900, 1, 1);
        private static readonly Random s_Rand = new();

        public static Func<T, Task> Create<T>(Func<T, Task> serviceRunner) where T : class
        {
            return serviceRunner;
        }

        public static string GenerateRandomString()
        {
            return Guid.NewGuid().ToFlatString();
        }

        public static DateTime GenerateRandomDateTime()
        {
            int range = (DateTime.Today - s_StartDate).Days;
            return s_StartDate.AddDays(s_Rand.Next(range))
                .AddHours(s_Rand.Next(0, 24))
                .AddMinutes(s_Rand.Next(0, 60))
                .AddSeconds(s_Rand.Next(0, 60));
        }
    }
}
