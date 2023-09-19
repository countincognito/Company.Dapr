using Destructurama.Attributed;

namespace Company.Microservice.Membership.Data.v1_0.Mobile
{
    [Serializable]
    public class RegisterRequestDto
        : RegisterRequestDtoBase
    {
        [NotLogged]
        public required string Password { get; set; }
    }
}
