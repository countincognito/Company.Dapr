using ProtoBuf;

namespace Company.Utility.Encryption.Data
{
    [ProtoContract]
    [Serializable]
    public class CreateKeysRequest
    {
        [ProtoMember(1)]
        public required string SymmetricKeyName { get; set; }

        [ProtoMember(2)]
        public required string AsymmetricKeyName { get; set; }
    }
}
