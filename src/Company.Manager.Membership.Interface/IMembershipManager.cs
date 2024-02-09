using Company.Manager.Membership.Data;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace Company.Manager.Membership.Interface
{
    [Service]
    public interface IMembershipManager
    {
        [Operation]
        Task<RegisterResponseBase> RegisterMemberAsync(RegisterRequestBase request, CallContext context = default);

        [Operation]
        Task<RegisterResponseBase> RegisterAccountAsync(RegisterRequestBase request, CallContext context = default);
    }
}