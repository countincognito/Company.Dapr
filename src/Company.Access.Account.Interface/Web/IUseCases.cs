using Company.Access.Account.Data.Web;
using ProtoBuf.Grpc;

namespace Company.Access.Account.Interface.Web
{
    public interface IUseCases
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request, CallContext context = default);
    }
}