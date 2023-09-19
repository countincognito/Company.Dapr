using Destructurama.Attributed;
using ProtoBuf;

namespace Company.Utility.Cache.Data
{
    [ProtoContract]
    [Serializable]
    public class SetCachedValueRequest
    {
        [ProtoMember(1)]
        public required string Key { get; set; }

        [NotLogged]
        [ProtoMember(2)]
        public required byte[] Data { get; set; }
    }
}
