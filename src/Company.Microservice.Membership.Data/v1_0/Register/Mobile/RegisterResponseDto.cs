namespace Company.Microservice.Membership.Data.v1_0.Mobile
{
    [Serializable]
    public class RegisterResponseDto
        : RegisterResponseDtoBase
    {
        public required string MobileMessage { get; set; }
    }
}
