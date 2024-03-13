using Company.Engine.Registration.Interface;
using NATS.Client.Core;

namespace Company.iFX.Nats.TestWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _Logger;

        public Worker(ILogger<Worker> logger)
        {
            _Logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var engine = new RegistrationEngineNatsService();
            string? natsUrl = Environment.GetEnvironmentVariable("NATS_URL") ?? "127.0.0.1:4222";
            NatsOpts natsOpts = natsUrl is null ? NatsOpts.Default : NatsOpts.Default with { Url = natsUrl };
            await engine.AddServiceEndpointsAsync("0.0.1", "Test service", natsOpts, cancellationToken: stoppingToken).ConfigureAwait(false);
        }
    }
}