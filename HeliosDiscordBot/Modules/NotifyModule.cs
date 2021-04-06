namespace HeliosDiscordBot.Modules
{
    using System.Threading.Tasks;
    using Discord.Commands;
    using HeliosDiscordBot.Repository;

    [Group("notify")]
    public class NotifyModule : ModuleBase<SocketCommandContext>
    {
        private readonly IDatabaseRepository _repo;

        public NotifyModule(IDatabaseRepository repo)
        {
            _repo = repo;
        }

        [Command("sunrise")]
        [Summary("Sets a notification time for sunrise")]
        public async Task NotifySunriseAsync(int minutes)
        {
            var channelId = Context.Channel.Id;
            var notification = await _repo.GetNotificationByChannelIdAsync(channelId);
            
            if (notification == null)
            {
                await ReplyAsync("You need to set a location first by using `!setlocation <latitude> <longitude>`. For example `!setlocation 37.8199286 -122.4795565`");
                return;
            }

            notification.NotifySunrise = minutes;
            notification.NextNotifySunriseUtc = null;
            await _repo.SaveNotificationAsync(notification);
            await ReplyAsync($"I will notify you of sunrise {minutes} minutes before it happens.");
        }

        [Command("sunset")]
        [Summary("Sets a notification time for sunset")]
        public async Task NotifySunsetAsync(int minutes)
        {
            var channelId = Context.Channel.Id;
            var notification = await _repo.GetNotificationByChannelIdAsync(channelId);
            
            if (notification == null)
            {
                await ReplyAsync("You need to set a location first by using `!setlocation <latitude> <longitude>`. For example `!setlocation 37.8199286 -122.4795565`");
                return;
            }

            notification.NotifySunset = minutes;
            notification.NextNotifySunsetUtc = null;
            await _repo.SaveNotificationAsync(notification);
            await ReplyAsync($"I will notify you of sunset {minutes} minutes before it happens.");
        }
    }
}
