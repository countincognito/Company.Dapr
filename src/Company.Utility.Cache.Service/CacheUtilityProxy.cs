using Company.iFX.Proxy;
using Company.Utility.Cache.Data;
using Company.Utility.Cache.Interface;
using ProtoBuf.Grpc;

namespace Company.Utility.Cache.Service
{
    public class CacheUtilityProxy
        : ICacheUtility
    {
        private readonly ICacheUtility m_Proxy;

        public CacheUtilityProxy()
        {
            m_Proxy = Proxy.Create<ICacheUtility>();
        }

        public async Task DeleteCachedValueAsync(
            DeleteCachedValueRequest request,
            CallContext context = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            await m_Proxy.DeleteCachedValueAsync(request, context);
        }

        public async Task<GetCachedValueResponse> GetCachedValueAsync(
            GetCachedValueRequest request,
            CallContext context = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return await m_Proxy.GetCachedValueAsync(request, context);
        }

        public async Task RefreshCachedValueAsync(
            RefreshCachedValueRequest request,
            CallContext context = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            await m_Proxy.RefreshCachedValueAsync(request, context);
        }

        public async Task SetCachedValueAsync(
            SetCachedValueRequest request,
            CallContext context = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            await m_Proxy.SetCachedValueAsync(request, context);
        }
    }
}
