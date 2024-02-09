using Castle.DynamicProxy;
using Zametek.Utility;

namespace Company.iFX.Nats
{
    public static class NatsClient
    {
        private static readonly ProxyGenerator s_ProxyGenerator = new();

        public static T Create<T>(string? natsUrl = null) where T : class
        {
            typeof(T).ThrowIfNotInterface();
            return s_ProxyGenerator.CreateInterfaceProxyWithoutTarget<T>(
                new AsyncNatsClientInterceptor<T>(natsUrl).ToInterceptor());
        }
    }
}
