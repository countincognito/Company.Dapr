using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Company.iFX.Container
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseiFXContainer(
            this IHostBuilder hostBuilder,
            string companyName = "*")
        {
            if (Configuration.Configuration.SystemUnderTest)
            {
                throw new InvalidOperationException("Cannot use iFX when system is under test");
            }

            var factoryProvider = new AutofacServiceProviderFactory(builder =>
            {
                builder.RegisterBuildCallback(scope => Container.OverrideScope(scope));
                Container.LoadAssemblies(companyName, builder);
                Container.LoadMapper(companyName, builder);
            });
            return hostBuilder.UseServiceProviderFactory(factoryProvider);
        }
    }
}