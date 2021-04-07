namespace HeliosDiscordBot
{
    using System;
    using HeliosDiscordBot.Repository;
    using HeliosDiscordBot.Settings;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo
                .Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}").CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).UseSerilog().UseWindowsService().ConfigureAppConfiguration(
                (hostContext, builder) =>
                {
                    if (hostContext.HostingEnvironment.IsDevelopment())
                    {
                        builder.AddUserSecrets<Program>();
                    }
                    builder.AddEnvironmentVariables("HeliosDiscordBot_");
                }).ConfigureServices(
                (hostContext, services) =>
                {
                    var config = hostContext.Configuration;
                    services.Configure<DiscordSettings>(config.GetSection(nameof(DiscordSettings)));
                    services.Configure<DatabaseSettings>(config.GetSection(nameof(DatabaseSettings)));

                    services.AddTransient<IDatabaseRepository, DatabaseRepository>();

                    services.AddSingleton(services);

                    services.AddHostedService<Worker>();
                });
    }
}
