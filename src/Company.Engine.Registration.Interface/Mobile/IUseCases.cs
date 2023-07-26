using Company.Engine.Registration.Data.Mobile;

namespace Company.Engine.Registration.Interface.Mobile
{
    public interface IUseCases
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    }
}