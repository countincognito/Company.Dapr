using Zametek.Utility;

namespace Company.iFX.Common
{
    public static class Addressing
    {
        public static string Microservice<I>()
        {
            typeof(I).ThrowIfNotInterface();
            return $@"/{Naming.Microservice<I>()}/{Naming.VolatilityName<I>()}";
        }

        public static string Component<I>()
        {
            typeof(I).ThrowIfNotInterface();
            return $@"/{Naming.ComponentName<I>()}/{Naming.VolatilityName<I>()}";
        }
    }
}
