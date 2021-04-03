namespace HeliosDiscordBot
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Discord;
    using Discord.WebSocket;
    using HeliosDiscordBot.Domain;
    using HeliosDiscordBot.Settings;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly DiscordSettings _discordSettings;
        private readonly DiscordSocketClient _client;

        public Worker(ILogger<Worker> logger, IOptions<DiscordSettings> discordSettings)
        {
            _logger = logger;
            _discordSettings = discordSettings.Value;
            _client = new DiscordSocketClient();
            _client.Log += Log;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("The current date and time is: {time}", DateTime.Now);
            _logger.LogInformation("Calculating next sunrise and sunset...");
            var solarCalculation = new SolarCalculation(32.8240344, -97.187775, DateTime.UtcNow);

            _logger.LogInformation("Sunrise today is at {sunriseTime}", solarCalculation.Sunrise.ToLocalTime());
            _logger.LogInformation("Sunset today is at {sunsetTime}", solarCalculation.Sunset.ToLocalTime());

            _logger.LogInformation("Logging into Discord...");
            await _client.LoginAsync(TokenType.Bot, _discordSettings.Token);
            await _client.StartAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private Task Log(LogMessage message)
        {
            _logger.LogInformation(message.ToString(prependTimestamp: false));
            return Task.CompletedTask;
        }
    }
}
