using Company.Access.Account.Data;
using Company.Access.Account.Interface;
using Company.iFX.Nats;
using Company.iFX.Proxy;
using NATS.Client.Core;
using ProtoBuf.Grpc;

namespace Company.Access.Account.Service
{
    public class AccountAccessProxy
        : NatsServiceBase, IAccountAccess
    {
        private readonly IAccountAccess m_Proxy;
        private readonly string? m_NatsUrl;

        public AccountAccessProxy(string? natsUrl)
        {
            m_Proxy = Proxy.Create<IAccountAccess>();
            m_NatsUrl = natsUrl;
        }

        private async Task<RegisterResponseBase?> RegisterFunction(
                   RegisterRequestBase? input,
                   CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(input);

            return await m_Proxy
                .RegisterAsync(input, cancellationToken)
                .ConfigureAwait(false);
        }

        #region IAccountAccess Members

        public async Task<RegisterResponseBase> RegisterAsync(
            RegisterRequestBase request,
            CallContext context = default)
        {
            // No null checks here because the only value that will
            // ever be passed in is null.

            NatsOpts natsOpts = m_NatsUrl is null ? NatsOpts.Default : NatsOpts.Default with { Url = m_NatsUrl };

            RegisterResponseBase? reply = await SubscribeAsync<IAccountAccess, RegisterRequestBase, RegisterResponseBase>(
                    RegisterFunction,
                    opts: natsOpts,
                    cancellationToken: context.CancellationToken)
                .ConfigureAwait(false);

            return reply!;
        }

        #endregion
    }
}
