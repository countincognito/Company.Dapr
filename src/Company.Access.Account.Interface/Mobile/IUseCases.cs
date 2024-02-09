using Company.Access.Account.Data.Mobile;
using ProtoBuf.Grpc;

namespace Company.Access.Account.Interface.Mobile
{
    public interface IUseCases
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request, CallContext context = default);
    }
}