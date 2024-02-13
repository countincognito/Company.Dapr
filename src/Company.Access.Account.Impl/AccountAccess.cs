using Company.Access.Account.Data;
using Company.Access.Account.Interface;
using Company.iFX.Proxy;
using ProtoBuf.Grpc;
using Serilog;
using Zametek.Utility.Logging;

namespace Company.Access.Account.Impl
{
    [DiagnosticLogging(LogActive.On)]
    public class AccountAccess
        : IAccountAccess
    {
        private readonly ILogger m_Logger;

        public AccountAccess()
        {
            m_Logger = Proxy.CreateLogger<IAccountAccess>();
        }

        public async Task<RegisterResponseBase> RegisterAsync(
            RegisterRequestBase registerRequest,
            [DiagnosticLogging(LogActive.Off)] CallContext context = default)
        {
            ArgumentNullException.ThrowIfNull(registerRequest);

            m_Logger.Information($"{nameof(RegisterAsync)} Invoked");
            m_Logger.Information($"{nameof(RegisterAsync)} {registerRequest}");

            return await UseCaseFactory<IAccountAccess, RegisterRequestBase, CallContext, RegisterResponseBase>
                .CallAsync(registerRequest, context)
                .ConfigureAwait(false);
        }
    }
}
