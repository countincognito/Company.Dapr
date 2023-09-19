using ProtoBuf;

namespace Company.Common.Data
{
    [ProtoContract]
    public class ErrorInfo
    {
        [ProtoMember(1)]
        public int Code { get; set; }

        [ProtoMember(2)]
        public required string Description { get; set; }
    }
}
