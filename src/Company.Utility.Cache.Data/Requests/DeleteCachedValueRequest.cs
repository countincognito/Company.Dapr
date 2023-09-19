using ProtoBuf;

namespace Company.Utility.Cache.Data
{
    [ProtoContract]
    [Serializable]
    public class DeleteCachedValueRequest
    {
        [ProtoMember(1)]
        public required string Key { get; set; }
    }
}
