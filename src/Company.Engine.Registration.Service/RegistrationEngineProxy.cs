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

        public async Task<RegisterResponseBase> RegisterAsync(
            RegisterRequestBase registerRequest,
            CallContext context = default)
        {
            return await m_Proxy.RegisterAsync(registerRequest, context);
        }
    }
}
