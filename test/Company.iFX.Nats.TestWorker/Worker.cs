using Company.Engine.Registration.Interface;

namespace Company.iFX.Nats.TestWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var engine = new RegistrationEngineNatsService();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await engine.SubscribeAllAsync<IRegistrationEngine>(cancellationToken: stoppingToken).ConfigureAwait(false);
            }
        }
    }
}