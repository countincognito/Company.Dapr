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
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var engine = new RegistrationEngineNatsService();

                await engine.Invoke<IRegistrationEngine>(stoppingToken).ConfigureAwait(false);
            }
        }
    }
}