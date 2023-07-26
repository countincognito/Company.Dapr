using Destructurama.Attributed;
using ProtoBuf;

namespace Company.Manager.Membership.Data.Mobile
{
    [ProtoContract]
    [Serializable]
    public class RegisterRequest
        : RegisterRequestBase
    {
        [NotLogged]
        [ProtoMember(1)]
        public string? Password { get; set; }
    }
}
