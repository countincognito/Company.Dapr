using ProtoBuf;

namespace Company.Utility.Encryption.Data
{
    [ProtoContract]
    [Serializable]
    public class RotateAsymmetricKeyRequest
    {
        [ProtoMember(1)]
        public Guid SymmetricKeyId { get; set; }
    }
}
