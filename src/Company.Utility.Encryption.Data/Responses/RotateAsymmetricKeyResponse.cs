using ProtoBuf;

namespace Company.Utility.Encryption.Data
{
    [ProtoContract]
    [Serializable]
    public class RotateAsymmetricKeyResponse
    {
        [ProtoMember(1)]
        public SymmetricKeyDefinition? SymmetricKeyDefinition { get; set; }

        [ProtoMember(2)]
        public AsymmetricKeyDefinition? AsymmetricKeyDefinition { get; set; }
    }
}
