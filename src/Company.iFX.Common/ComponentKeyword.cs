using Zametek.Utility;

namespace Company.iFX.Common
{
    public class ComponentKeyword
        : SafeEnumString<ComponentKeyword>
    {
        public static readonly ComponentKeyword Manager = new(nameof(Manager));
        public static readonly ComponentKeyword Engine = new(nameof(Engine));
        public static readonly ComponentKeyword Access = new(nameof(Access));
        public static readonly ComponentKeyword Resource = new(nameof(Resource));
        public static readonly ComponentKeyword Utility = new(nameof(Utility));
        public static readonly ComponentKeyword UseCases = new(nameof(UseCases));

        public ComponentKeyword(string value)
            : base(value)
        {
        }
    }
}
