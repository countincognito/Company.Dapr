using Company.iFX.Common;
using NATS.Client.Serializers.Json;
using System.Text.Json.Serialization;

namespace Company.iFX.Nats
{
    public static class PolymorphicJsonSerializer
    {
        public static NatsJsonSerializer<T> Create<T>()
        {
            return new NatsJsonSerializer<T>(
                 new System.Text.Json.JsonSerializerOptions
                 {
                     DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                     TypeInfoResolver = new PolymorphicTypeResolver(),
                 });
        }
    }
}
