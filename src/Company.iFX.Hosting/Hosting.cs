using Company.iFX.Container;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Reflection;

namespace Company.iFX.Hosting
{
    public static class Hosting
    {
        public static IHostBuilder CreateGenericBuilder(
            string[] args,
            params string[] companyNames)
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            Debug.Assert(assemblyLocation is not null);

            string? contentRoot = Path.GetDirectoryName(assemblyLocation);
            Debug.Assert(contentRoot is not null);

            return Host.CreateDefaultBuilder(args)
                .UseiFXContainer(companyNames)
                .UseContentRoot(contentRoot);
        }

        public static IHostBuilder AddTestHost(
            this IHostBuilder host,
            Func<Task> testAction)
        {
            return host.ConfigureServices((hostContext, services) =>
            {
                if (testAction is not null)
                {
                    services.AddHostedService(serviceProvider =>
                    {
                        IHostApplicationLifetime? hostApplicationLifetime = serviceProvider.GetService<IHostApplicationLifetime>();
                        Debug.Assert(hostApplicationLifetime is not null);

                        return new InProc.TestFromBackgroundService(hostApplicationLifetime, testAction);
                    });
                }
            });
        }

        public static Task RunAsync(this IHostBuilder host)
        {
            return host.RunConsoleAsync();
        }

        public static Task Stop(this IHostBuilder host)
        {
            return host.Stop();
        }
    }
}
