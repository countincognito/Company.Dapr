using Company.Engine.Registration.Data.Mobile;
using ProtoBuf.Grpc;

namespace Company.Engine.Registration.Interface.Mobile
{
    public interface IUseCases
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request, CallContext context = default);
    }
}