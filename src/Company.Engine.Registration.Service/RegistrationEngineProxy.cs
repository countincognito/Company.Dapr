using Company.Engine.Registration.Data;
using Company.Engine.Registration.Interface;
using Company.iFX.Proxy;
using ProtoBuf.Grpc;

namespace Company.Engine.Registration.Service
{
    public class RegistrationEngineProxy
        : IRegistrationEngine
    {
        private readonly IRegistrationEngine m_Proxy;

        public RegistrationEngineProxy()
        {
            m_Proxy = Proxy.Create<IRegistrationEngine>();
        }

        public async Task<RegisterResponseBase> RegisterMemberAsync(
            RegisterRequestBase registerRequest,
            CallContext context = default)
        {
            if (registerRequest is null)
            {
                throw new ArgumentNullException(nameof(registerRequest));
            }

            return await m_Proxy.RegisterMemberAsync(registerRequest, context).ConfigureAwait(false);
        }

        public async Task<RegisterResponseBase> RegisterAccountAsync(
            RegisterRequestBase registerRequest,
            CallContext context = default)
        {
            if (registerRequest is null)
            {
                throw new ArgumentNullException(nameof(registerRequest));
            }

            return await m_Proxy.RegisterAccountAsync(registerRequest, context).ConfigureAwait(false);
        }
    }
}
