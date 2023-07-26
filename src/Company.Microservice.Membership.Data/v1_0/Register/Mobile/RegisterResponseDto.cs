namespace Company.Microservice.Membership.Data.v1_0.Mobile
{
    [Serializable]
    public class RegisterResponseDto
        : RegisterResponseDtoBase
    {
        public string? MobileMessage { get; set; }
    }
}
