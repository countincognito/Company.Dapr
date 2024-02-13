using Company.Access.Account.Interface;
using Company.iFX.Configuration;

namespace Company.Access.Account.Service
{
    public class Worker
        : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string? natsUrl = Configuration.Current.Setting<string>("NATS:URL");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var access = new AccountAccessProxy(natsUrl);

                await access.Invoke<IAccountAccess>(stoppingToken).ConfigureAwait(false);
            }
        }
    }
}