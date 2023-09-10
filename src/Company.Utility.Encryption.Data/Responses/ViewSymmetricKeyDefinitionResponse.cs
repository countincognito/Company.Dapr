using ProtoBuf;

namespace Company.Utility.Encryption.Data
{
    [ProtoContract]
    [Serializable]
    public class ViewSymmetricKeyDefinitionResponse
    {
        [ProtoMember(1)]
        public SymmetricKeyDefinition? SymmetricKeyDefinition { get; set; }
    }
}
