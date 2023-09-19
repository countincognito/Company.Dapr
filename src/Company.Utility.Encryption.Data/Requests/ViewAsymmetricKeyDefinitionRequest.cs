using ProtoBuf;

namespace Company.Utility.Encryption.Data
{
    [ProtoContract]
    [Serializable]
    public class ViewAsymmetricKeyDefinitionRequest
    {
        [ProtoMember(1)]
        public required string Name { get; set; }

        [ProtoMember(2)]
        public required string Version { get; set; }
    }
}
