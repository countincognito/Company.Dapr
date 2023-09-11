using Destructurama.Attributed;
using ProtoBuf;

namespace Company.Utility.Cache.Data
{
    [ProtoContract]
    [Serializable]
    public class SetCachedValueRequest
    {
        [ProtoMember(1)]
        public string? Key { get; set; }

        [NotLogged]
        [ProtoMember(2)]
        public byte[]? Data { get; set; }
    }
}
