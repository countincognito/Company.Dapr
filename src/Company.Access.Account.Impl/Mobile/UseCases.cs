using Company.Access.Account.Data.Mobile;
using Company.Access.Account.Interface.Mobile;
using Company.iFX.Proxy;
using ProtoBuf.Grpc;
using Serilog;
using Zametek.Utility;
using Zametek.Utility.Logging;

namespace Company.Access.Account.Impl.Mobile
{
    [DiagnosticLogging(LogActive.On)]
    public class UseCases
        : IUseCases
    {
        private readonly ILogger m_Logger;

        public UseCases()
        {
            m_Logger = Proxy.CreateLogger<IUseCases>();
        }

        public Task<RegisterResponse> RegisterAsync(
            RegisterRequest registerRequest,
            CallContext context = default)
        {
            ArgumentNullException.ThrowIfNull(registerRequest);

            m_Logger.Information($"{nameof(RegisterAsync)} Invoked");
            m_Logger.Information($"{nameof(RegisterAsync)} {registerRequest.Name}");

            RegisterResponse response = new()
            {
                Name = registerRequest.Name,
                MobileMessage = registerRequest.Password,
            };

            return Task.FromResult(response);
        }
    }
}
