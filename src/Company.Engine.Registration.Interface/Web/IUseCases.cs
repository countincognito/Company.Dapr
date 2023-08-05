using Company.Engine.Registration.Data.Web;
using ProtoBuf.Grpc;

namespace Company.Engine.Registration.Interface.Web
{
    public interface IUseCases
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request, CallContext context = default);
    }
}