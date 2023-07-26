using Company.Common.Data;
using ProtoBuf;

namespace Company.Access.User.Data
{
    [ProtoInclude(11, typeof(Mobile.RegisterResponse))]
    [ProtoInclude(12, typeof(Web.RegisterResponse))]
    [ProtoContract]
    [Serializable]
    public abstract class RegisterResponseBase
        : Response
    {
        [ProtoMember(1)]
        public string? Name { get; set; }
    }
}
