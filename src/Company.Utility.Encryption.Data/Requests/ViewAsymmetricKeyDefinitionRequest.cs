using ProtoBuf;

namespace Company.Utility.Encryption.Data
{
    [ProtoContract]
    [Serializable]
    public class ViewAsymmetricKeyDefinitionRequest
    {
        [ProtoMember(1)]
        public string? Name { get; set; }

        [ProtoMember(2)]
        public string? Version { get; set; }
    }
}
