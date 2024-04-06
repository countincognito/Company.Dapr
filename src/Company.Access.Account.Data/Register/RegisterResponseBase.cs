using Company.Common.Data;
using ProtoBuf;

namespace Company.Access.Account.Data
{
    [ProtoInclude(101, typeof(Mobile.RegisterResponse))]
    [ProtoInclude(102, typeof(Web.RegisterResponse))]
    [ProtoContract]
    [Serializable]
    public abstract class RegisterResponseBase
        : Response
    {
        [ProtoMember(1)]
        public required string Name { get; set; }
    }
}
