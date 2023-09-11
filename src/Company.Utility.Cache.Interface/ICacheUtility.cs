using Company.Utility.Cache.Data;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace Company.Utility.Cache.Interface
{
    [Service]
    public interface ICacheUtility
    {
        [Operation]
        Task<GetCachedValueResponse> GetCachedValueAsync(GetCachedValueRequest request, CallContext context = default);

        [Operation]
        Task RefreshCachedValueAsync(RefreshCachedValueRequest request, CallContext context = default);

        [Operation]
        Task DeleteCachedValueAsync(DeleteCachedValueRequest request, CallContext context = default);

        [Operation]
        Task SetCachedValueAsync(SetCachedValueRequest request, CallContext context = default);
    }
}
