using ProtoBuf;

namespace Company.Utility.Encryption.Data
{
    [ProtoContract]
    [Serializable]
    public class ViewAsymmetricKeyDefinitionResponse
    {
        [ProtoMember(1)]
        public AsymmetricKeyDefinition? AsymmetricKeyDefinition { get; set; }
    }
}
