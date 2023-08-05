using Company.Access.User.Data.Mobile;
using ProtoBuf.Grpc;

namespace Company.Access.User.Interface.Mobile
{
    public interface IUseCases
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request, CallContext context = default);
    }
}