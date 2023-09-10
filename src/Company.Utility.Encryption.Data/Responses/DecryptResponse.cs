using ProtoBuf;
using Zametek.Utility.Logging;

namespace Company.Utility.Encryption.Data
{
    [ProtoContract]
    [Serializable]
    public class DecryptResponse
    {
        [NoLogging]
        [ProtoMember(1)]
        public byte[]? Data { get; set; }
    }
}
