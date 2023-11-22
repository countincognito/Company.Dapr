using Company.Engine.Registration.Data;
using Company.Engine.Registration.Interface;
using Company.iFX.Common;
using NATS.Client.Core;
using NATS.Client.Serializers.Json;
using ProtoBuf.Grpc;
using System.Text.Json.Serialization;
using Zametek.Utility;

namespace Company.iFX.Nats.TestWorker
{
    public class RegistrationEngineNatsService
        : IRegistrationEngine
    {
        public async Task<RegisterResponseBase> RegisterAsync(
            RegisterRequestBase request,
            CallContext context = default)
        {
            CancellationToken cancellationToken = context.CancellationToken;

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

            await foreach (NatsMsg<RegisterRequestBase> msg in nats.SubscribeAsync<RegisterRequestBase>(subject, serializer: requestSerializer))
            {
                RegisterResponseBase? response = null;

                msg.Data!.TypeSwitchOn()
                    .Case<Engine.Registration.Data.Mobile.RegisterRequest>(x =>
                    {
                        Console.WriteLine($"Received mobile request: {x.Name}");

                        response = new Engine.Registration.Data.Mobile.RegisterResponse
                        { 
                            Name = @$"This is the Mobile response name: email = {x.Email}",
                            MobileMessage = "Mobile Message",
                        };
                    })
                    .Case<Engine.Registration.Data.Web.RegisterRequest>(x =>
                    {
                        Console.WriteLine($"Received web request: {x.Name}");

                        response = new Engine.Registration.Data.Web.RegisterResponse
                        {
                            Name = @$"This is the Web response name: DOB = {x.DateOfBirth}",
                            WebMessage = "Web Message",
                        };
                    })
                    .Default(_ => throw new InvalidOperationException());

                await msg.ReplyAsync(
                    response,
                    serializer: replySerializer,
                    cancellationToken: cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }
            }

            throw new InvalidOperationException();
        }

    }
}