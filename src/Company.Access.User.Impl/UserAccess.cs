using Company.Access.User.Data;
using Company.Access.User.Interface;
using Company.iFX.Proxy;
using ProtoBuf.Grpc;
using Serilog;
using Zametek.Utility.Logging;

namespace Company.Access.User.Impl
{
    [DiagnosticLogging(LogActive.On)]
    public class UserAccess
        : IUserAccess
    {
        private readonly ILogger m_Logger;

        public UserAccess()
        {
            m_Logger = Proxy.CreateLogger<IUserAccess>();
        }

        public async Task<RegisterResponseBase> RegisterAsync(
            RegisterRequestBase registerRequest,
            CallContext context = default)
        {
            m_Logger.Information($"{nameof(RegisterAsync)} Invoked");
            m_Logger.Information($"{nameof(RegisterAsync)} {registerRequest}");
            return await UseCaseFactory<IUserAccess, RegisterRequestBase, RegisterResponseBase>.CallAsync(registerRequest);
        }
    }
}
