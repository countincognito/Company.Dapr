using Company.iFX.Common;
using Company.iFX.Grpc;
using Dapr.Client;
using Grpc.Core.Interceptors;
using ProtoBuf.Grpc.Client;
using System.Diagnostics;

namespace Company.iFX.Dapr
{
    public static class TrackingContextDaprClient
    {
        public static T Create<T>() where T : class
        {
            Debug.Assert(typeof(T).IsInterface);
            return DaprClient
                .CreateInvocationInvoker(Naming.AppId<T>())
                .Intercept(new TrackingContextInterceptor())
                .CreateGrpcService<T>();
        }
    }
}
