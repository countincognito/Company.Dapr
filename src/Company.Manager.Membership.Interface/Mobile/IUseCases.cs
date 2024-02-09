using Company.Manager.Membership.Data.Mobile;
using ProtoBuf.Grpc;

namespace Company.Manager.Membership.Interface.Mobile
{
    public interface IUseCases
    {
        Task<RegisterResponse> RegisterMemberAsync(RegisterRequest request, CallContext context = default);

        Task<RegisterResponse> RegisterAccountAsync(RegisterRequest request, CallContext context = default);
    }
}