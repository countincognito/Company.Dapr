using ProtoBuf;
using Zametek.Utility.Logging;

namespace Company.Utility.Encryption.Data
{
    [ProtoContract]
    [Serializable]
    public class EncryptRequest
    {
        [ProtoMember(1)]
        public Guid SymmetricKeyId { get; set; }

        [NoLogging]
        [ProtoMember(2)]
        public byte[]? Data { get; set; }
    }
}
