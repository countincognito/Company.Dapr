using ProtoBuf;

namespace Company.Utility.Encryption.Data
{
    [ProtoContract]
    [Serializable]
    public class ViewAsymmetricKeyDefinitionResponse
    {
        [ProtoMember(1)]
        public required AsymmetricKeyDefinition AsymmetricKeyDefinition { get; set; }
    }
}
