using ProtoBuf;

namespace Company.Access.User.Data.Web
{
    [ProtoContract]
    [Serializable]
    public class RegisterRequest
        : RegisterRequestBase
    {
        [ProtoMember(1)]
        public DateTime DateOfBirth { get; set; }
    }
}
