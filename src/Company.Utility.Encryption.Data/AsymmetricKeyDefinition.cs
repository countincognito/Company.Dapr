using ProtoBuf;

namespace Company.Utility.Encryption.Data
{
    [ProtoContract]
    [Serializable]
    public class AsymmetricKeyDefinition
    {
        [ProtoMember(1)]
        public string? Id { get; set; }

        [ProtoMember(2)]
        public required string Name { get; set; }

        [ProtoMember(3)]
        public required string Version { get; set; }

        [ProtoMember(4)]
        public bool? IsEnabled { get; set; }

        [ProtoMember(5)]
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
