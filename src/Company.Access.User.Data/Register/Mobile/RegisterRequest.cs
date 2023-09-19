using Destructurama.Attributed;
using ProtoBuf;

namespace Company.Access.User.Data.Mobile
{
    [ProtoContract]
    [Serializable]
    public class RegisterRequest
        : RegisterRequestBase
    {
        [NotLogged]
        [ProtoMember(1)]
        public required string Password { get; set; }
    }
}
