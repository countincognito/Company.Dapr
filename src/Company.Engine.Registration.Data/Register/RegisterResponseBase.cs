using Company.Common.Data;
using ProtoBuf;

namespace Company.Engine.Registration.Data
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
