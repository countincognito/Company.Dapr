using Company.Common.Data;
using ProtoBuf;

namespace Company.Manager.Membership.Data
{
    [ProtoInclude(11, typeof(Mobile.RegisterResponse))]
    [ProtoInclude(12, typeof(Web.RegisterResponse))]
    [ProtoContract]
    [Serializable]
    public abstract class RegisterResponseBase
        : Response
    {
        [ProtoMember(1)]
        public required string Name { get; set; }
    }
}
