using Company.Access.User.Data.Web;
using Company.Access.User.Interface.Web;
using Company.iFX.Proxy;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;
using Zametek.Utility.Cache;
using Zametek.Utility.Logging;

namespace Company.Access.User.Impl.Web
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

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            m_Logger.Information($"{nameof(RegisterAsync)} Invoked");
            m_Logger.Information($"{nameof(RegisterAsync)} {registerRequest.Name}");

            string webMessage = string.Empty;

            try
            {
                DateTime? responseDob = await m_CacheUtility.GetAsync<DateTime?>(registerRequest.Name);

                if (responseDob is null)
                {
                    m_Logger.Warning(@"No DOB currently stored for name: {@Name}", registerRequest.Name);
                    await m_CacheUtility.SetAsync(
                        registerRequest.Name,
                        registerRequest.DateOfBirth,
                        m_DefaultDistributedCacheEntryOptions);
                    responseDob = await m_CacheUtility.GetAsync<DateTime?>(registerRequest.Name);
                }
                else
                {
                    m_Logger.Warning(@"Email already stored for name: {@Name}", registerRequest.Name);
                }

                webMessage = responseDob?.ToString() ?? @"No DOB";
            }
            catch (Exception ex)
            {
                webMessage = "Something weird happened!";
                m_Logger.Error(ex, @"Unable to cache email for name: {@Name}", registerRequest.Name);
            }

            RegisterResponse response = new()
            {
                Name = registerRequest.Name,
                WebMessage = webMessage,
            };

            return response;
        }
    }
}
