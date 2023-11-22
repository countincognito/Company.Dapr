//using Company.iFX.Common;
//using Company.Utility.Cache.Data;
//using Company.Utility.Cache.Interface;
//using NATS.Client.Core;
//using NATS.Client.Serializers.Json;
//using ProtoBuf.Grpc;
//using System.Text.Json.Serialization;

//namespace Company.iFX.Nats.TestConsole
//{
//    public class CacheUtilityNatsProxy
//        : ICacheUtility
//    {
//        public async Task DeleteCachedValueAsync(
//            DeleteCachedValueRequest request,
//            CallContext context = default)
//        {
//            if (request is null)
//            {
//                throw new ArgumentNullException(nameof(request));
//            }

//            throw new NotImplementedException();
//        }

//        public async Task<GetCachedValueResponse> GetCachedValueAsync(
//            GetCachedValueRequest request,
//            CallContext context = default)
//        {
//            if (request is null)
//            {
//                throw new ArgumentNullException(nameof(request));
//            }

//            await using var nats = new NatsConnection();

//            string subject = Addressing.Subject<ICacheUtility>();

//            var requestSerializer = new NatsJsonSerializer<GetCachedValueRequest>(
//                 new System.Text.Json.JsonSerializerOptions
//                 {
//                     DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
//                     TypeInfoResolver = new PolymorphicTypeResolver(),
//                 });

//            var replySerializer = new NatsJsonSerializer<GetCachedValueResponse>(
//                 new System.Text.Json.JsonSerializerOptions
//                 {
//                     DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
//                     TypeInfoResolver = new PolymorphicTypeResolver(),
//                 });

//            NatsMsg<GetCachedValueResponse> reply =
//                await nats.RequestAsync(
//                    subject,
//                    request,
//                    requestSerializer: requestSerializer,
//                    replySerializer: replySerializer);

//            return reply.Data!;
//        }

//        public async Task RefreshCachedValueAsync(
//            RefreshCachedValueRequest request,
//            CallContext context = default)
//        {
//            if (request is null)
//            {
//                throw new ArgumentNullException(nameof(request));
//            }

//            throw new NotImplementedException();
//        }

//        public async Task SetCachedValueAsync(
//            SetCachedValueRequest request,
//            CallContext context = default)
//        {
//            if (request is null)
//            {
//                throw new ArgumentNullException(nameof(request));
//            }

//            throw new NotImplementedException();
//        }
//    }
//}
