using ProtoBuf;

namespace Company.Common.Data
{
    [ProtoContract]
    public class Response
    {
        [ProtoMember(1)]
        public ErrorInfo? Error { get; set; }
    }
}
