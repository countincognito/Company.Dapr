using Company.iFX.Common;
using System.Runtime.CompilerServices;
using Zametek.Utility;

namespace Company.iFX.Nats
{
    public static class Addressing
    {
        public static string Subject<I>([CallerMemberName] string memberName = "")
        {
            typeof(I).ThrowIfNotInterface();
            if (string.IsNullOrEmpty(memberName))
            {
                throw new ArgumentNullException(nameof(memberName));
            }

            return $@"{Naming.ComponentName<I>()}.{Naming.VolatilityName<I>()}.{memberName}";
        }
    }
}
