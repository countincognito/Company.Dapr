using AutoMapper;
using Company.Access.User.Interface;
using Company.Engine.Registration.Data.Mobile;
using Company.Engine.Registration.Interface.Mobile;
using Company.iFX.Proxy;
using ProtoBuf.Grpc;
using Serilog;
using Zametek.Utility.Logging;

namespace Company.Engine.Registration.Impl.Mobile
{
    [DiagnosticLogging(LogActive.On)]
    public class UseCases
        : IUseCases
    {
        private readonly ILogger m_Logger;
        private readonly IMapper m_Mapper;

        public UseCases()
        {
            m_Logger = Proxy.CreateLogger<IUseCases>();
            m_Mapper = iFX.Container.Container.GetService<IMapper>();
        }

        public async Task<RegisterResponse> RegisterAsync(
            RegisterRequest registerRequest,
            CallContext context = default)
        {
            if (registerRequest is null)
            {
                throw new ArgumentNullException(nameof(registerRequest));
            }

            m_Logger.Information($"{nameof(RegisterAsync)} Invoked");
            m_Logger.Information($"{nameof(RegisterAsync)} {registerRequest.Name}");

            Access.User.Data.RegisterRequestBase userRegisterRequest =
                m_Mapper.Map<Access.User.Data.RegisterRequestBase>(registerRequest);

            IUserAccess userAccess = Proxy.Create<IUserAccess>();
            Access.User.Data.RegisterResponseBase userResponse = await userAccess.RegisterAsync(userRegisterRequest, context.CancellationToken);

            RegisterResponse registerResponse = m_Mapper.Map<RegisterResponse>(userResponse);

            return registerResponse;
        }
    }
}
