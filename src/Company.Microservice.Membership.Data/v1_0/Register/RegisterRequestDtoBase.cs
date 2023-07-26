namespace Company.Microservice.Membership.Data.v1_0
{
    [Serializable]
    public abstract class RegisterRequestDtoBase
    {
        public string? Name { get; set; }

        public string? Email { get; set; }
    }
}
