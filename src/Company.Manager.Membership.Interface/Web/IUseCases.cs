using Company.Manager.Membership.Data.Web;
using ProtoBuf.Grpc;

namespace Company.Manager.Membership.Interface.Web
{
    public interface IUseCases
    {
        Task<RegisterResponse> RegisterMemberAsync(RegisterRequest request, CallContext context = default);
    }
}