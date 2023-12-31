﻿using Company.Engine.Registration.Data;
using Company.Engine.Registration.Interface;
using ProtoBuf.Grpc;

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

            RegisterResponseBase? reply = await NatsClientHelper
                .CallAsync<IRegistrationEngine, RegisterRequestBase, RegisterResponseBase>(
                    request,
                    cancellationToken: context.CancellationToken)
                .ConfigureAwait(false);

            return reply!;
        }
    }
}
