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

        public const string ActivityTraceIdName = @"activitytraceid";
        public const string ActivitySpanIdName = @"activityspanid";

        public const string TrackingCallChainTag = @"trackingcontext.callchainid";
        public const string ServiceNamespaceTag = @"service.sourcecode.namespace";
        public const string ServiceTypeTag = @"service.sourcecode.type";
        public const string ServiceMethodTag = @"service.sourcecode.method";
    }
}
