using ProtoBuf;

namespace Company.Access.User.Data.Web
{
    [ProtoContract]
    [Serializable]
    public class RegisterResponse
        : RegisterResponseBase
    {
        [ProtoMember(1)]
        public required string WebMessage { get; set; }
    }
}
