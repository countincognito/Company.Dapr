using Company.Engine.Registration.Data;
using Company.Engine.Registration.Interface;
using Company.iFX.Common;
using NATS.Client.Core;
using NATS.Client.Serializers.Json;
using ProtoBuf.Grpc;
using System.Text.Json.Serialization;

namespace Company.iFX.Nats.TestConsole
{
    public class RegistrationEngineNatsProxy
        : IRegistrationEngine
    {
        public async Task<RegisterResponseBase> RegisterAsync(
            RegisterRequestBase request,
            CallContext context = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            await using var nats = new NatsConnection();

            string subject = Addressing.Subject<IRegistrationEngine>();

            var requestSerializer = new NatsJsonSerializer<RegisterRequestBase>(
                 new System.Text.Json.JsonSerializerOptions
                 {
                     DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                     TypeInfoResolver = new PolymorphicTypeResolver(),
                 });

            var replySerializer = new NatsJsonSerializer<RegisterResponseBase>(
                 new System.Text.Json.JsonSerializerOptions
                 {
                     DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                     TypeInfoResolver = new PolymorphicTypeResolver(),
                 });

            NatsMsg<RegisterResponseBase> reply =
                await nats.RequestAsync(
                    subject,
                    request,
                    requestSerializer: requestSerializer,
                    replySerializer: replySerializer);

            return reply.Data!;
        }

    }
}
