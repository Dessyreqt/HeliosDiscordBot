namespace HeliosDiscordBot.Modules
{
    using System;
    using System.Threading.Tasks;
    using Discord.Commands;
    using HeliosDiscordBot.Domain;
    using HeliosDiscordBot.Repository;

    public class SunriseModule : ModuleBase<SocketCommandContext>
    {
        private readonly IDatabaseRepository _repo;

        public SunriseModule(IDatabaseRepository repo)
        {
            _repo = repo;
        }

        [Command("sunrise")]
        [Summary("Tells you your next sunrise")]
        public async Task SunriseAsync()
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
            
            while (calculation.Sunrise < currentDate)
            {
                date = date.AddDays(1);
                calculation = new SolarCalculation(notification.Latitude, notification.Longitude, date);
            }

            var convertedDate = calculation.Sunrise.ToTimeZone(notification.Timezone);
            var minutesToSunset = (int)Math.Round((calculation.Sunrise - currentDate).TotalMinutes);


            await ReplyAsync($"Your next sunrise is at {convertedDate:h:mm:ss tt}, which is about {minutesToSunset} minutes from now.");
        }
    }
}
