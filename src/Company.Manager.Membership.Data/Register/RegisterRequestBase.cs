using ProtoBuf;

namespace Company.Manager.Membership.Data
{
    [ProtoInclude(11, typeof(Mobile.RegisterRequest))]
    [ProtoInclude(12, typeof(Web.RegisterRequest))]
    [ProtoContract]
    [Serializable]
    public abstract class RegisterRequestBase
    {
        [ProtoMember(1)]
        public string? Name { get; set; }

        [ProtoMember(2)]
        public string? Email { get; set; }
    }
}
