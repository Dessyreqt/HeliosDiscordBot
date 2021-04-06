namespace HeliosDiscordBot.Modules
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Discord.Commands;
    using HeliosDiscordBot.Repository;

    public class SetTimezoneModule : ModuleBase<SocketCommandContext>
    {
        private readonly IDatabaseRepository _repo;

        public SetTimezoneModule(IDatabaseRepository repo)
        {
            _repo = repo;
        }

        [Command("settimezone")]
        [Summary("Sets a time zone for future sunrise/sunset notifications.")]
        public async Task SetTimezoneAsync([Remainder]string timezone)
        {
            var channelId = Context.Channel.Id;
            var notification = await _repo.GetNotificationByChannelIdAsync(channelId);

            if (notification == null)
            {
                await ReplyAsync("You need to set a location first by using `!setlocation <latitude> <longitude>`. For example, `!setlocation 37.8199286 -122.4795565`.");
                return;
            }

            var timezoneTrimmed = timezone.Trim();
            var foundZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(zone => zone.DisplayName == timezoneTrimmed);

            if (foundZone == null)
            {
                await ReplyAsync("I don't recognize that time zone, please use `!listtimezones` and copy and paste the closest time zone to the one you want into the command `!settimezone <time zone>`. For example, `!settimezone (UTC+08:00) Perth`");
                return;
            }

            notification.Timezone = foundZone.DisplayName;
            await _repo.SaveNotificationAsync(notification);
            var currentDate = DateTime.UtcNow;
            var currentTimeInZone = TimeZoneInfo.ConvertTimeFromUtc(currentDate, foundZone);
            await ReplyAsync($"Changed your time zone to {foundZone.DisplayName}. Your current local time should be {currentTimeInZone:h:mm:ss tt}.");
        }
    }
}
