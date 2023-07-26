using AutoMapper;
using Company.Access.User.Interface;
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
        private readonly IMapper m_Mapper;

        public UseCases()
        {
            m_Logger = Proxy.CreateLogger<IUseCases>();
            m_Mapper = iFX.Container.Container.GetService<IMapper>();
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            m_Logger.Information($"{nameof(RegisterAsync)} Invoked");
            m_Logger.Information($"{nameof(RegisterAsync)} {registerRequest.Name}");

            Access.User.Data.RegisterRequestBase userRegisterRequest =
                m_Mapper.Map<Access.User.Data.RegisterRequestBase>(registerRequest);

            IUserAccess userAccess = Proxy.Create<IUserAccess>();
            Access.User.Data.RegisterResponseBase userResponse = await userAccess.RegisterAsync(userRegisterRequest);

            RegisterResponse registerResponse = m_Mapper.Map<RegisterResponse>(userResponse);

            return registerResponse;
        }
    }
}
