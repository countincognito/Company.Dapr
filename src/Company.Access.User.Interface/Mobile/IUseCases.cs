using Company.Access.User.Data.Mobile;

namespace Company.Access.User.Interface.Mobile
{
    public interface IUseCases
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    }
}