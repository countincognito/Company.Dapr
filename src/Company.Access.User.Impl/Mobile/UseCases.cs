using Company.Access.User.Data.Mobile;
using Company.Access.User.Interface.Mobile;
using Company.iFX.Proxy;
using Microsoft.Extensions.Caching.Distributed;
using ProtoBuf.Grpc;
using Serilog;
using Zametek.Utility.Cache;
using Zametek.Utility.Logging;

namespace Company.Access.User.Impl.Mobile
{
    [DiagnosticLogging(LogActive.On)]
    public class UseCases
        : IUseCases
    {
        private readonly ILogger m_Logger;
        private readonly ICacheUtility m_CacheUtility;

        private readonly DistributedCacheEntryOptions m_DefaultDistributedCacheEntryOptions = new()
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        };

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
                string password = await m_CacheUtility.GetAsync<string>(registerRequest.Name, context.CancellationToken);

                if (string.IsNullOrWhiteSpace(password))
                {
                    m_Logger.Warning(@"No password currently stored for name: {@Name}", registerRequest.Name);
                    await m_CacheUtility.SetAsync(
                        registerRequest.Name,
                        registerRequest.Password,
                        m_DefaultDistributedCacheEntryOptions,
                        context.CancellationToken);
                    password = await m_CacheUtility.GetAsync<string>(registerRequest.Name, context.CancellationToken);
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
