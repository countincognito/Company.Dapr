namespace Company.Access.User.Data.Db
{
    [Serializable]
    public class NameValueSet
    {
        public required string Name { get; set; }

        public required string Value { get; set; }

        public Guid SymmetricKeyId { get; set; }

        public required byte[] EncryptedValue { get; set; }
    }
}