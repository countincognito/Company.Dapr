using ProtoBuf;
using Zametek.Utility.Logging;

namespace Company.Utility.Encryption.Data
{
    [ProtoContract]
    [Serializable]
    public class EncryptResponse
    {
        [NoLogging]
        [ProtoMember(1)]
        public required byte[] EncryptedData { get; set; }
    }
}
