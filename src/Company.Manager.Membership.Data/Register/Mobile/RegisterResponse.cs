using ProtoBuf;

namespace Company.Manager.Membership.Data.Mobile
{
    [ProtoContract]
    [Serializable]
    public class RegisterResponse
        : RegisterResponseBase
    {
        [ProtoMember(1)]
        public required string MobileMessage { get; set; }
    }
}
