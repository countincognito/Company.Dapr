using System.Diagnostics;

namespace Company.iFX.Common
{
    public class Constant
    {
        public const int NamespaceSize = 4;
        public const int CompanyIndex = 0;
        public const int ComponentIndex = 1;
        public const int VolatilityIndex = 2;
        public const int TypeIndex = 3;

        public const int NumberOfServiceMethodParameters = 2;

        public const string DiscriminatorName = @"$type";
        public const string ApiVersionString = @"Api-Version";

        public const string ActivityTraceIdName = nameof(ActivityTraceId);
        public const string ActivitySpanIdName = nameof(ActivitySpanId);

        public const string Namespace = nameof(Namespace);
        public const string TargetType = nameof(TargetType);
        public const string Method = nameof(Method);
    }
}
