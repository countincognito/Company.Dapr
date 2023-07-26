using Company.Engine.Registration.Data;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace Company.Engine.Registration.Interface
{
    [Service]
    public interface IRegistrationEngine
    {
        [Operation]
        Task<RegisterResponseBase> RegisterAsync(RegisterRequestBase request, CallContext context = default);
    }
}
