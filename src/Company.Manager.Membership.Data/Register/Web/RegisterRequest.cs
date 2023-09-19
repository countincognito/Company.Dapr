using ProtoBuf;

namespace Company.Manager.Membership.Data.Web
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
