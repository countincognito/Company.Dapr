namespace Company.Microservice.Membership.Data.v1_0.Web
{
    [Serializable]
    public class RegisterResponseDto
        : RegisterResponseDtoBase
    {
        public required string WebMessage { get; set; }
    }
}
