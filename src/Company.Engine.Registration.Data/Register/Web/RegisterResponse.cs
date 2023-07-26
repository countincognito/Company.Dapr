using ProtoBuf;

namespace Company.Engine.Registration.Data.Web
{
    [ProtoContract]
    [Serializable]
    public class RegisterResponse
        : RegisterResponseBase
    {
        [ProtoMember(1)]
        public string? WebMessage { get; set; }
    }
}
