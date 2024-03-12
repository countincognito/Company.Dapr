using Company.Access.Account.Interface;
using Company.iFX.Configuration;
using NATS.Client.Core;

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
            NatsOpts natsOpts = natsUrl is null ? NatsOpts.Default : NatsOpts.Default with { Url = natsUrl };
            var access = new AccountAccessProxy();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await access.SubscribeAllAsync<IAccountAccess>(opts: natsOpts, cancellationToken: stoppingToken).ConfigureAwait(false);
            }
        }
    }
}