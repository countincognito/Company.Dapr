using System.Collections.ObjectModel;
using Zametek.Utility;

namespace Company.iFX.Common
{
    public class ConventionKeyword
        : SafeEnumString<ConventionKeyword>
    {
        public static readonly ReadOnlyCollection<ConventionKeyword> All;

        public static readonly ConventionKeyword Data = new(nameof(Data));
        public static readonly ConventionKeyword Impl = new(nameof(Impl));
        public static readonly ConventionKeyword Interface = new(nameof(Interface));
        public static readonly ConventionKeyword Service = new(nameof(Service));
        public static readonly ConventionKeyword Microservice = new(nameof(Microservice));

        public ConventionKeyword(string value)
            : base(value)
        {
        }

        static ConventionKeyword()
        {
            All = SafeEnumStringHelper.GetAll<ConventionKeyword>();
        }
    }
}
