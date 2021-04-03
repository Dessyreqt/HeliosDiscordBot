namespace HeliosDiscordBot
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using HeliosDiscordBot.Domain;
    using HeliosDiscordBot.Settings;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceCollection _services;
        private readonly DiscordSettings _discordSettings;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public Worker(ILogger<Worker> logger, IServiceCollection services, IOptions<DiscordSettings> discordSettings)
        {
            _logger = logger;
            _services = services;
            _discordSettings = discordSettings.Value;
            _client = new DiscordSocketClient();
            _client.Log += LogAsync;
            _commands = new CommandService();
            _commands.Log += LogAsync;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            OutputSunriseSunset();
            await InstallCommandsAsync();

            _logger.LogInformation("Logging into Discord...");
            await _client.LoginAsync(TokenType.Bot, _discordSettings.Token);
            await _client.StartAsync();
            await _client.SetGameAsync("DM me !help to start");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
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
            if (message == null)
            {
                return;
            }

            // Only process commands in DMs
            if (!(message.Channel is SocketDMChannel))
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
