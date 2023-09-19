using Destructurama.Attributed;
using ProtoBuf;

namespace Company.Utility.Cache.Data
{
    [ProtoContract]
    [Serializable]
    public class GetCachedValueResponse
    {
        [NotLogged]
        [ProtoMember(1)]
        public required byte[] Data { get; set; }
    }
}
