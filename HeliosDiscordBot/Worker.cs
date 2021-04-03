namespace HeliosDiscordBot
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using HeliosDiscordBot.Domain;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("The current date and time is: {time}", DateTime.Now);
            _logger.LogInformation("Calculating next sunrise and sunset...");
            var solarCalculation = new SolarCalculation(32.8240344, -97.187775, DateTime.UtcNow);

            _logger.LogInformation("Sunrise today is at {sunriseTime}", solarCalculation.Sunrise.ToLocalTime());
            _logger.LogInformation("Sunset today is at {sunsetTime}", solarCalculation.Sunset.ToLocalTime());

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
