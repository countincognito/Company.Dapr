using Company.iFX.Common;
using Company.iFX.Grpc;
using Dapr.Client;
using Grpc.Core.Interceptors;
using ProtoBuf.Grpc.Client;
using Zametek.Utility;

namespace Company.iFX.Dapr
{
    public static class TrackingContextDaprClient
    {
        public static T Create<T>() where T : class
        {
            typeof(T).ThrowIfNotInterface();
            return DaprClient
                .CreateInvocationInvoker(Naming.AppId<T>())
                .Intercept(new TrackingContextGrpcInterceptor())
                .CreateGrpcService<T>();
        }
    }
}
