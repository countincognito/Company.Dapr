using Company.Access.User.Data.Web;

namespace Company.Access.User.Interface.Web
{
    public interface IUseCases
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    }
}