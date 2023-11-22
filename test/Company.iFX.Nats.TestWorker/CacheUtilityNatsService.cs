//using Company.iFX.Common;
//using Company.Utility.Cache.Data;
//using Company.Utility.Cache.Interface;
//using NATS.Client.Core;
//using NATS.Client.Serializers.Json;
//using ProtoBuf.Grpc;
//using System.Text.Json.Serialization;
//using Zametek.Utility;

//namespace Company.iFX.Nats.TestWorker
//{
//    public class CacheUtilityNatsService
//        : ICacheUtility
//    {
//        public Task DeleteCachedValueAsync(
//            DeleteCachedValueRequest request,
//            CallContext context = default)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<GetCachedValueResponse> GetCachedValueAsync(
//            GetCachedValueRequest request,
//            CallContext context = default)
//        {
//            CancellationToken cancellationToken = context.CancellationToken;

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

//            await foreach (NatsMsg<GetCachedValueRequest> msg in nats.SubscribeAsync<GetCachedValueRequest>(subject, serializer: requestSerializer))
//            {
//                Console.WriteLine($"Received request: {msg.Data!.Key}");

//                var response = new GetCachedValueResponse
//                {
//                    Data = @$"Your message was {msg.Data!.Key}".StringToByteArray(),
//                };

//                await msg.ReplyAsync(
//                    response,
//                    serializer: replySerializer,
//                    cancellationToken: cancellationToken);

//                if (cancellationToken.IsCancellationRequested)
//                {
//                    return null;
//                }
//            }

//            throw new InvalidOperationException();
//        }

//        public Task RefreshCachedValueAsync(
//            RefreshCachedValueRequest request,
//            CallContext context = default)
//        {
//            throw new NotImplementedException();
//        }

//        public Task SetCachedValueAsync(
//            SetCachedValueRequest request,
//            CallContext context = default)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}