using Company.Access.Account.Data.Web;
using Company.Access.Account.Interface.Web;
using Company.iFX.Proxy;
using Google.Protobuf.WellKnownTypes;
using ProtoBuf.Grpc;
using Serilog;
using System.Diagnostics;
using Zametek.Utility;
using Zametek.Utility.Logging;

namespace Company.Access.Account.Impl.Web
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
                WebMessage = registerRequest.DateOfBirth.ToString("u") ?? @"No DOB",
            };

            return Task.FromResult(response);
        }
    }
}
