using ProtoBuf;

namespace Company.Access.User.Data.Web
{
    [ProtoContract]
    [Serializable]
    public class RegisterResponse
        : RegisterResponseBase
    {
        [ProtoMember(1)]
        public string? WebMessage { get; set; }
    }
}
