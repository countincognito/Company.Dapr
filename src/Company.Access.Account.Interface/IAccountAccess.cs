using Company.Access.Account.Data;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace Company.Access.Account.Interface
{
    [Service]
    public interface IAccountAccess
    {
        [Operation]
        Task<RegisterResponseBase> RegisterAsync(RegisterRequestBase request, CallContext context = default);
    }
}
