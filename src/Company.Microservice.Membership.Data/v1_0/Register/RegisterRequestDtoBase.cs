namespace Company.Microservice.Membership.Data.v1_0
{
    [Serializable]
    public abstract class RegisterRequestDtoBase
    {
        public required string Name { get; set; }

        public required string Email { get; set; }
    }
}
