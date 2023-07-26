using Company.Manager.Membership.Data.Web;

namespace Company.Manager.Membership.Interface.Web
{
    public interface IUseCases
    {
        Task<RegisterResponse> RegisterMemberAsync(RegisterRequest request);
    }
}