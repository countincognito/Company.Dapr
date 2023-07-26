using Company.Manager.Membership.Data.Mobile;

namespace Company.Manager.Membership.Interface.Mobile
{
    public interface IUseCases
    {
        Task<RegisterResponse> RegisterMemberAsync(RegisterRequest request);
    }
}