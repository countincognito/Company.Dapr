using Company.Access.User.Data;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace Company.Access.User.Interface
{
    [Service]
    public interface IUserAccess
    {
        [Operation]
        Task<RegisterResponseBase> RegisterAsync(RegisterRequestBase request, CallContext context = default);
    }
}
