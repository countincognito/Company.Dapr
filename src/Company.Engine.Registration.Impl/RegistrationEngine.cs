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

        public async Task<RegisterResponseBase> RegisterMemberAsync(
            RegisterRequestBase registerRequest,
            [DiagnosticLogging(LogActive.Off)] CallContext context = default)
        {
            ArgumentNullException.ThrowIfNull(registerRequest);

            m_Logger.Information($"{nameof(RegisterMemberAsync)} Invoked");
            m_Logger.Information($"{nameof(RegisterMemberAsync)} {registerRequest}");

            return await UseCaseFactory<IRegistrationEngine, RegisterRequestBase, CallContext, RegisterResponseBase>
                .CallAsync(registerRequest, context)
                .ConfigureAwait(false);
        }

        public async Task<RegisterResponseBase> RegisterAccountAsync(
            RegisterRequestBase registerRequest,
            [DiagnosticLogging(LogActive.Off)] CallContext context = default)
        {
            ArgumentNullException.ThrowIfNull(registerRequest);

            m_Logger.Information($"{nameof(RegisterAccountAsync)} Invoked");
            m_Logger.Information($"{nameof(RegisterAccountAsync)} {registerRequest}");
            return await UseCaseFactory<IRegistrationEngine, RegisterRequestBase, CallContext, RegisterResponseBase>
                .CallAsync(registerRequest, context)
                .ConfigureAwait(false);
        }
    }
}
