using Company.Access.Account.Data;
using Company.Access.Account.Interface;
using Company.iFX.Nats;
using Company.iFX.Proxy;
using ProtoBuf.Grpc;

namespace Company.Access.Account.Service
{
    public class AccountAccessProxy
        : NatsServiceBase<IAccountAccess>, IAccountAccess
    {
        private readonly IAccountAccess m_Proxy;

        public AccountAccessProxy()
        {
            m_Proxy = Proxy.Create<IAccountAccess>();
        }

        #region IAccountAccess Members

        public async Task<RegisterResponseBase> RegisterAsync(
            RegisterRequestBase request,
            CallContext context = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            return await m_Proxy
                .RegisterAsync(request, context)
                .ConfigureAwait(false);
        }

        #endregion
    }
}
