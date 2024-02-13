using AutoMapper;
using Company.Access.Account.Interface;
using Company.Access.User.Interface;
using Company.Engine.Registration.Data.Web;
using Company.Engine.Registration.Interface.Web;
using Company.iFX.Proxy;
using ProtoBuf.Grpc;
using Serilog;
using Zametek.Utility.Logging;

namespace Company.Engine.Registration.Impl.Web
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

        public async Task<RegisterResponse> RegisterMemberAsync(
            RegisterRequest registerRequest,
            CallContext context = default)
        {
            ArgumentNullException.ThrowIfNull(registerRequest);

            m_Logger.Information($"{nameof(RegisterMemberAsync)} Invoked");
            m_Logger.Information($"{nameof(RegisterMemberAsync)} {registerRequest.Name}");

            Access.User.Data.RegisterRequestBase userRegisterRequest =
                m_Mapper.Map<Access.User.Data.RegisterRequestBase>(registerRequest);

            IUserAccess userAccess = Proxy.Create<IUserAccess>();
            Access.User.Data.RegisterResponseBase userResponse = await userAccess
                .RegisterAsync(userRegisterRequest, context.CancellationToken)
                .ConfigureAwait(false);

            RegisterResponse registerResponse = m_Mapper.Map<RegisterResponse>(userResponse);

            return registerResponse;
        }

        public async Task<RegisterResponse> RegisterAccountAsync(
            RegisterRequest registerRequest,
            CallContext context = default)
        {
            ArgumentNullException.ThrowIfNull(registerRequest);

            m_Logger.Information($"{nameof(RegisterAccountAsync)} Invoked");
            m_Logger.Information($"{nameof(RegisterAccountAsync)} {registerRequest.Name}");

            Access.Account.Data.RegisterRequestBase accountRegisterRequest =
                m_Mapper.Map<Access.Account.Data.RegisterRequestBase>(registerRequest);

            IAccountAccess accountAccess = Proxy.Create<IAccountAccess>();
            Access.Account.Data.RegisterResponseBase accountResponse = await accountAccess
                .RegisterAsync(accountRegisterRequest, context.CancellationToken)
                .ConfigureAwait(false);

            RegisterResponse registerResponse = m_Mapper.Map<RegisterResponse>(accountResponse);

            return registerResponse;
        }
    }
}
