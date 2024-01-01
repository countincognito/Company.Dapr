using Company.Engine.Registration.Data;
using Company.Engine.Registration.Interface;
using Company.iFX.Proxy;
using ProtoBuf.Grpc;
using Serilog;
using Zametek.Utility.Logging;

namespace Company.Engine.Registration.Impl
{
    [DiagnosticLogging(LogActive.On)]
    public class RegistrationEngine
        : IRegistrationEngine
    {
        private readonly ILogger m_Logger;

        public RegistrationEngine()
        {
            m_Logger = Proxy.CreateLogger<IRegistrationEngine>();
        }

        public async Task<RegisterResponseBase> RegisterAsync(
            RegisterRequestBase registerRequest,
            [DiagnosticLogging(LogActive.Off)] CallContext context = default)
        {
            if (registerRequest is null)
            {
                throw new ArgumentNullException(nameof(registerRequest));
            }

            m_Logger.Information($"{nameof(RegisterAsync)} Invoked");
            m_Logger.Information($"{nameof(RegisterAsync)} {registerRequest}");
            return await UseCaseFactory<IRegistrationEngine, RegisterRequestBase, CallContext, RegisterResponseBase>
                .CallAsync(registerRequest, context)
                .ConfigureAwait(false);
        }
    }
}
