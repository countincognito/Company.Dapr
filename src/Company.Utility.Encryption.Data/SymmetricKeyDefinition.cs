using ProtoBuf;

namespace Company.Utility.Encryption.Data
{
    [ProtoContract]
    [Serializable]
    public class SymmetricKeyDefinition
    {
        [ProtoMember(1)]
        public Guid Id { get; set; }

        [ProtoMember(2)]
        public string? Name { get; set; }

        [ProtoMember(3)]
        public bool IsEnabled { get; set; }

        [ProtoMember(4)]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
