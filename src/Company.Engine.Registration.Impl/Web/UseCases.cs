using Company.Engine.Registration.Data.Web;
using Company.Engine.Registration.Interface.Web;
using Company.iFX.Proxy;
using Serilog;
using Zametek.Utility.Logging;

namespace Company.Engine.Registration.Impl.Web
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
                WebMessage = registerRequest.DateOfBirth.GetValueOrDefault().ToString(),
            };

            return Task.FromResult(response);
        }
    }
}
