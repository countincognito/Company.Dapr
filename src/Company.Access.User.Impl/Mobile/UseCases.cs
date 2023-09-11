using Company.Access.User.Data.Mobile;
using Company.Access.User.Interface.Mobile;
using Company.iFX.Proxy;
using Company.Utility.Cache.Data;
using Company.Utility.Cache.Interface;
using ProtoBuf.Grpc;
using Serilog;
using Zametek.Utility;
using Zametek.Utility.Logging;

namespace Company.Access.User.Impl.Mobile
{
    [DiagnosticLogging(LogActive.On)]
    public class UseCases
        : IUseCases
    {
        private readonly ILogger m_Logger;
        private readonly ICacheUtility m_CacheUtility;

        public UseCases()
        {
            m_Logger = Proxy.CreateLogger<IUseCases>();
            m_CacheUtility = Proxy.Create<ICacheUtility>(m_Logger);
        }

        public async Task<RegisterResponse> RegisterAsync(
            RegisterRequest registerRequest,
            CallContext context = default)
        {
            m_Logger.Information($"{nameof(RegisterAsync)} Invoked");
            m_Logger.Information($"{nameof(RegisterAsync)} {registerRequest.Name}");

            string mobileMessage = string.Empty;

            try
            {
                var getRequest = new GetCachedValueRequest
                {
                    Key = registerRequest.Name,
                };

                GetCachedValueResponse getResponse = await m_CacheUtility.GetCachedValueAsync(getRequest, context.CancellationToken);
                string password = getResponse?.Data?.ByteArrayToObject<string>() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(password))
                {
                    m_Logger.Warning(@"No password currently stored for name: {@Name}", registerRequest.Name);

                    var setRequest = new SetCachedValueRequest
                    {
                        Key = registerRequest.Name,
                        Data = registerRequest.Password.ObjectToByteArray(),
                    };

                    await m_CacheUtility.SetCachedValueAsync(setRequest, context.CancellationToken);

                    getRequest = new GetCachedValueRequest
                    {
                        Key = registerRequest.Name,
                    };

                    getResponse = await m_CacheUtility.GetCachedValueAsync(getRequest, context.CancellationToken);
                    password = getResponse?.Data?.ByteArrayToObject<string>() ?? string.Empty;
                }
                else
                {
                    m_Logger.Warning(@"Password already stored for name: {@Name}", registerRequest.Name);
                }

                mobileMessage = password;
            }
            catch (Exception ex)
            {
                mobileMessage = "Something weird happened!";
                m_Logger.Error(ex, @"Unable to cache password for name: {@Name}", registerRequest.Name);
            }

            RegisterResponse response = new()
            {
                Name = registerRequest.Name,
                MobileMessage = mobileMessage,
            };

            return response;
        }
    }
}
