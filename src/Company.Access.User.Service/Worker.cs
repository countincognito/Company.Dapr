using Company.iFX.Configuration;
using NATS.Client.Core;

namespace Company.Access.User.Service
{
    public class Worker
        : BackgroundService
    {
        private readonly ILogger<Worker> _Logger;

        public Worker(ILogger<Worker> logger)
        {
            _Logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _Logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            var access = new UserAccessProxy();
            string? natsUrl = Configuration.Current.Setting<string>("NATS:URL");
            NatsOpts natsOpts = natsUrl is null ? NatsOpts.Default : NatsOpts.Default with { Url = natsUrl };
            await access.AddServiceEndpointsAsync("0.0.1", "User Access Service", natsOpts, cancellationToken: stoppingToken).ConfigureAwait(false);
        }
    }
}