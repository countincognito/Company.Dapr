namespace Company.Microservice.Membership.Data.v1_0.Web
{
    [Serializable]
    public class RegisterRequestDto
        : RegisterRequestDtoBase
    {
        public DateTime DateOfBirth { get; set; }
    }
}
