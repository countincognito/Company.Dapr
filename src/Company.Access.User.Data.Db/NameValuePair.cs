namespace Company.Access.User.Data.Db
{
    [Serializable]
    public class NameValuePair
    {
        public string? Name { get; set; }

        public byte[]? Value { get; set; }
    }
}