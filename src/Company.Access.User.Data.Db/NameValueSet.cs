namespace Company.Access.User.Data.Db
{
    [Serializable]
    public class NameValueSet
    {
        public string? Name { get; set; }

        public string? Value { get; set; }

        public Guid SymmetricKeyId { get; set; }

        public byte[]? EncryptedValue { get; set; }
    }
}