using ProtoBuf;
using Zametek.Utility.Logging;

namespace Company.Utility.Encryption.Data
{
    [ProtoContract]
    [Serializable]
    public class DecryptRequest
    {
        [ProtoMember(1)]
        public Guid SymmetricKeyId { get; set; }

        [NoLogging]
        [ProtoMember(2)]
        public required byte[] EncryptedData { get; set; }
    }
}
