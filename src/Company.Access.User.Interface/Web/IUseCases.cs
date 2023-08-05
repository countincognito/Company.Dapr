using Company.Access.User.Data.Web;
using ProtoBuf.Grpc;

namespace Company.Access.User.Interface.Web
{
    public interface IUseCases
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request, CallContext context = default);
    }
}