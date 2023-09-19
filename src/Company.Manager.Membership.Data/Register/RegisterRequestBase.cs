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
        public required string Name { get; set; }

        [ProtoMember(2)]
        public required string Email { get; set; }
    }
}
