using ProtoBuf;

namespace Company.Utility.Encryption.Data
{
    [ProtoContract]
    [Serializable]
    public class CreateKeysRequest
    {
        [ProtoMember(1)]
        public string? SymmetricKeyName { get; set; }

        [ProtoMember(2)]
        public string? AsymmetricKeyName { get; set; }
    }
}
