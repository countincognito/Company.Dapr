using ProtoBuf;

namespace Company.Manager.Membership.Data.Mobile
{
    [ProtoContract]
    [Serializable]
    public class RegisterResponse
        : RegisterResponseBase
    {
        [ProtoMember(1)]
        public string? MobileMessage { get; set; }
    }
}
