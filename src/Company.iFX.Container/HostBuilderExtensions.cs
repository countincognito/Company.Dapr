using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Company.iFX.Container
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseiFXContainer(
            this IHostBuilder hostBuilder,
            params string[] companyNames)
        {
            if (Configuration.Configuration.SystemUnderTest)
            {
                throw new InvalidOperationException("Cannot use iFX when system is under test");
            }

            var factoryProvider = new AutofacServiceProviderFactory(builder =>
            {
                builder.RegisterBuildCallback(scope => Container.OverrideScope(scope));

                foreach (string companyName in companyNames)
                {
                    Container.LoadAssemblies(companyName, builder);
                    Container.LoadMapper(companyName, builder);
                }
            });
            return hostBuilder.UseServiceProviderFactory(factoryProvider);
        }
    }
}