namespace HeliosDiscordBot.Modules
{
    using System;
    using System.Threading.Tasks;
    using Discord.Commands;
    using HeliosDiscordBot.Domain;
    using HeliosDiscordBot.Repository;

    public class SunsetModule : ModuleBase<SocketCommandContext>
    {
        private readonly IDatabaseRepository _repo;

        public SunsetModule(IDatabaseRepository repo)
        {
            _repo = repo;
        }

        [Command("sunset")]
        [Summary("Tells you your next sunset")]
        public async Task SunsetAsync()
        {
            var channelId = Context.Channel.Id;
            var notification = await _repo.GetNotificationByChannelIdAsync(channelId);

            if (notification == null)
            {
                await ReplyAsync("You need to set a location first by using `!setlocation <latitude> <longitude>`. For example, `!setlocation 37.8199286 -122.4795565`.");
                return;
            }

            var currentDate = DateTime.UtcNow;
            var date = DateTime.UtcNow.Date.AddHours(12).AddDays(-1);
            var calculation = new SolarCalculation(notification.Latitude, notification.Longitude, date);

            while (calculation.Sunset < currentDate)
            {
                date = date.AddDays(1);
                calculation = new SolarCalculation(notification.Latitude, notification.Longitude, date);
            }

            var convertedDate = calculation.Sunset.ToTimeZone(notification.Timezone);
            var minutesToSunset = (int)Math.Round((calculation.Sunset - currentDate).TotalMinutes);

            await ReplyAsync($"Your next sunset is at {convertedDate:h:mm:ss tt}, which is about {minutesToSunset} minutes from now.");
        }
    }
}
