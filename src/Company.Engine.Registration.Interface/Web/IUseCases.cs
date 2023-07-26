using Company.Engine.Registration.Data.Web;

namespace Company.Engine.Registration.Interface.Web
{
    public interface IUseCases
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    }
}