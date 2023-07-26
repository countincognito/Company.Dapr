using Microsoft.Extensions.Hosting;

namespace Company.iFX.Hosting.InProc
{
    public class TestFromBackgroundService
        : BackgroundService
    {
        private readonly IHostApplicationLifetime m_AppLifetime;
        private readonly Func<Task> m_TestAction;

        public TestFromBackgroundService(
            IHostApplicationLifetime appLifetime,
            Func<Task> testAction)
        {
            m_AppLifetime = appLifetime;
            m_TestAction = testAction;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await m_TestAction.Invoke();
            m_AppLifetime.StopApplication();
        }
    }
}
