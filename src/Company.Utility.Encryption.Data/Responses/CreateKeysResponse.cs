using ProtoBuf;

namespace Company.Utility.Encryption.Data
{
    [ProtoContract]
    [Serializable]
    public class CreateKeysResponse
    {
        [ProtoMember(1)]
        public required SymmetricKeyDefinition SymmetricKeyDefinition { get; set; }

        [ProtoMember(2)]
        public required AsymmetricKeyDefinition AsymmetricKeyDefinition { get; set; }
    }
}
