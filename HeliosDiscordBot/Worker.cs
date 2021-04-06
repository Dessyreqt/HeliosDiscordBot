namespace HeliosDiscordBot
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Commands;
    using Discord.Rest;
    using Discord.WebSocket;
    using HeliosDiscordBot.Domain;
    using HeliosDiscordBot.Repository;
    using HeliosDiscordBot.Settings;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceCollection _services;
        private readonly IDatabaseRepository _repo;
        private readonly DiscordSettings _discordSettings;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        private bool _ready = false;

        public Worker(ILogger<Worker> logger, IServiceCollection services, IDatabaseRepository repo, IOptions<DiscordSettings> discordSettings)
        {
            _logger = logger;
            _services = services;
            _repo = repo;
            _discordSettings = discordSettings.Value;
            _client = new DiscordSocketClient();
            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _commands = new CommandService();
            _commands.Log += LogAsync;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                OutputSunriseSunset();
                await InstallCommandsAsync();

                _logger.LogInformation("Logging into Discord...");
                await _client.LoginAsync(TokenType.Bot, _discordSettings.Token);
                await _client.StartAsync();

                while (!_ready)
                {
                    await Task.Delay(100, stoppingToken);
                }

                await _client.SetGameAsync("DM me !help to start");
                await SendMessage(827804539749924874, "Goliath online.");

                while (!stoppingToken.IsCancellationRequested)
                {
                    await SetNextNotificationTimesAsync();
                    await Task.Delay(1000, stoppingToken);
                }
            }
            finally
            {
                await _client.StopAsync();
            }
        }

        private async Task SendMessage(ulong channelId, string message)
        {
            var rawChannel = await _client.Rest.GetChannelAsync(channelId);
            var channel = rawChannel as IDMChannel;

            if (channel != null)
            {
                await channel.SendMessageAsync(message);
            }
        }

        private async Task SetNextNotificationTimesAsync()
        {
            var currentDate = DateTime.UtcNow;
            var notifications = await _repo.GetUnsetNotificationsAsync();

            foreach (var notification in notifications)
            {
                if (notification.NotifySunrise != null && notification.NextNotifySunriseUtc == null)
                {
                    UpdateSunrise(notification, currentDate);
                }

                if (notification.NotifySunset != null && notification.NextNotifySunsetUtc == null)
                {
                    UpdateSunset(notification, currentDate);
                }

                await _repo.SaveNotificationAsync(notification);
            }
        }

        private void UpdateSunrise(Notification notification, DateTime currentDate)
        {
            // start one day back in case
            var date = DateTime.UtcNow.Date.AddHours(12).AddDays(-1);
            var calculation = new SolarCalculation(notification.Latitude, notification.Longitude, date);
            
            while (calculation.Sunrise.AddMinutes(-notification.NotifySunrise.Value) < currentDate)
            {
                date = date.AddDays(1);
                calculation = new SolarCalculation(notification.Latitude, notification.Longitude, date);
            }

            notification.NextNotifySunriseUtc = calculation.Sunrise.AddMinutes(-notification.NotifySunrise.Value);
        }

        private void UpdateSunset(Notification notification, DateTime currentDate)
        {
            // start one day back in case
            var date = DateTime.UtcNow.Date.AddHours(12).AddDays(-1);
            var calculation = new SolarCalculation(notification.Latitude, notification.Longitude, date);
            
            while (calculation.Sunset.AddMinutes(-notification.NotifySunset.Value) < currentDate)
            {
                date = date.AddDays(1);
                calculation = new SolarCalculation(notification.Latitude, notification.Longitude, date);
            }

            notification.NextNotifySunsetUtc = calculation.Sunset.AddMinutes(-notification.NotifySunset.Value);
        }

        private Task ReadyAsync()
        {
            _ready = true;
            return Task.CompletedTask;
        }

        private async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services.BuildServiceProvider());
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;

            // Only process commands in DMs
            if (!(message?.Channel is SocketDMChannel))
            {
                return;
            }

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)) || message.Author.IsBot)
            {
                return;
            }

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commands.ExecuteAsync(context, argPos, _services.BuildServiceProvider());
        }

        private void OutputSunriseSunset()
        {
            _logger.LogInformation("The current date and time is: {time}", DateTime.Now);
            _logger.LogInformation("Calculating next sunrise and sunset...");
            var solarCalculation = new SolarCalculation(32.8240344, -97.187775, DateTime.UtcNow);

            _logger.LogInformation("Sunrise today is at {sunriseTime}", solarCalculation.Sunrise.ToLocalTime());
            _logger.LogInformation("Sunset today is at {sunsetTime}", solarCalculation.Sunset.ToLocalTime());
        }

        private Task LogAsync(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                _logger.LogInformation($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()} failed to execute in {cmdException.Context.Channel}.");
                _logger.LogError(cmdException, message.ToString(prependTimestamp: false));
            }

            _logger.LogInformation($"[General/{message.Severity}] {message.ToString(prependTimestamp: false)}");
            return Task.CompletedTask;
        }
    }
}
