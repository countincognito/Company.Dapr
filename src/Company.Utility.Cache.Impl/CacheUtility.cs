using AutoMapper;
using Company.iFX.Proxy;
using Company.Utility.Cache.Data;
using Company.Utility.Cache.Interface;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using ProtoBuf.Grpc;
using Serilog;
using Zametek.Utility.Logging;

namespace Company.Utility.Cache.Impl
{
    [DiagnosticLogging(LogActive.On)]
    public class CacheUtility
        : ICacheUtility
    {
        private readonly ILogger m_Logger;
        private readonly Zametek.Utility.Cache.ICacheUtility m_CacheUtility;
        private readonly IMapper m_Mapper;
        private readonly CacheOptions m_CacheOptions;

        public CacheUtility()
        {
            m_Logger = Proxy.CreateLogger<ICacheUtility>();
            m_CacheUtility = Proxy.Create<Zametek.Utility.Cache.ICacheUtility>(m_Logger);
            m_Mapper = iFX.Container.Container.GetService<IMapper>();
            m_CacheOptions = iFX.Container.Container.GetService<IOptions<CacheOptions>>().Value;
        }

        public async Task<GetCachedValueResponse> GetCachedValueAsync(
            GetCachedValueRequest request,
            CallContext context = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            m_Logger.Information($"{nameof(GetCachedValueAsync)} Invoked");

            Zametek.Utility.Cache.GetCachedValueResponse response = await m_CacheUtility
                .GetCachedValueAsync(m_Mapper.Map<Zametek.Utility.Cache.GetCachedValueRequest>(request), context.CancellationToken)
                .ConfigureAwait(false);

            if (response is null)
            {
                return new GetCachedValueResponse
                {
                    Data = Array.Empty<byte>()
                };
            }

            return m_Mapper.Map<GetCachedValueResponse>(response);
        }

        public async Task RefreshCachedValueAsync(
            RefreshCachedValueRequest request,
            CallContext context = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            m_Logger.Information($"{nameof(RefreshCachedValueAsync)} Invoked");

            await m_CacheUtility
                .RefreshCachedValueAsync(m_Mapper.Map<Zametek.Utility.Cache.RefreshCachedValueRequest>(request), context.CancellationToken)
                .ConfigureAwait(false);
        }

        public async Task DeleteCachedValueAsync(
            DeleteCachedValueRequest request,
            CallContext context = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            m_Logger.Information($"{nameof(DeleteCachedValueAsync)} Invoked");

            await m_CacheUtility
                .DeleteCachedValueAsync(m_Mapper.Map<Zametek.Utility.Cache.DeleteCachedValueRequest>(request), context.CancellationToken)
                .ConfigureAwait(false);
        }

        public async Task SetCachedValueAsync(
            SetCachedValueRequest request,
            CallContext context = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            m_Logger.Information($"{nameof(SetCachedValueAsync)} Invoked");

            var setCacheValueRequest = m_Mapper.Map<Zametek.Utility.Cache.SetCachedValueRequest>(request);

            setCacheValueRequest.Options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(m_CacheOptions.AbsoluteExpirationInMinutes),
            };

            await m_CacheUtility
                .SetCachedValueAsync(setCacheValueRequest, context.CancellationToken)
                .ConfigureAwait(false);
        }
    }
}
