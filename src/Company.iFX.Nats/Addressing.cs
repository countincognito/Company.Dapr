using Company.iFX.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Company.iFX.Nats
{
    public static class Addressing
    {
        public static string Subject<I>([CallerMemberName] string memberName = "")
        {
            Debug.Assert(typeof(I).IsInterface);
            if (string.IsNullOrEmpty(memberName))
            {
                throw new ArgumentNullException(nameof(memberName));
            }

            return $@"{Naming.ComponentName<I>()}.{Naming.VolatilityName<I>()}.{memberName}";
        }
    }
}
