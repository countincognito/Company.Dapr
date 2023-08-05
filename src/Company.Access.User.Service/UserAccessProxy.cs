using Company.Access.User.Data;
using Company.Access.User.Interface;
using Company.iFX.Proxy;
using ProtoBuf.Grpc;

namespace Company.Access.User.Service
{
    public class UserAccessProxy
        : IUserAccess
    {
        private readonly IUserAccess m_Proxy;

        public UserAccessProxy()
        {
            m_Proxy = Proxy.Create<IUserAccess>();
        }

        public async Task<RegisterResponseBase> RegisterAsync(
            RegisterRequestBase registerRequest,
            CallContext context = default)
        {
            return await m_Proxy.RegisterAsync(registerRequest, context);
        }
    }
}
