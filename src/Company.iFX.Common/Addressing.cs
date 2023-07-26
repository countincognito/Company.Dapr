using System.Diagnostics;

namespace Company.iFX.Common
{
    public static class Addressing
    {
        public static string Microservice<I>()
        {
            Debug.Assert(typeof(I).IsInterface);
            return $@"/{Naming.Microservice<I>()}/{Naming.VolatilityName<I>()}";
        }

        public static string Component<I>()
        {
            Debug.Assert(typeof(I).IsInterface);
            return $@"/{Naming.ComponentName<I>()}/{Naming.VolatilityName<I>()}";
        }
    }
}
