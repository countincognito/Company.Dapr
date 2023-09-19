namespace Company.Microservice.Membership.Data.v1_0
{
    [Serializable]
    public abstract class RegisterResponseDtoBase
    {
        public required string Name { get; set; }
    }
}
