using Company.Access.User.Data.Mobile;
using Company.Access.User.Interface.Mobile;
using Company.iFX.Proxy;
using Serilog;
using Zametek.Utility.Logging;

namespace Company.Access.User.Impl.Mobile
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

        public Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            m_Logger.Information($"{nameof(RegisterAsync)} Invoked");
            m_Logger.Information($"{nameof(RegisterAsync)} {registerRequest.Name}");

            RegisterResponse response = new()
            {
                Name = registerRequest.Name,
                MobileMessage = registerRequest.Password
            };

            return Task.FromResult(response);
        }
    }
}
