namespace HeliosDiscordBot.Modules
{
    using System.Threading.Tasks;
    using Discord.Commands;
    using HeliosDiscordBot.Repository;

    [Group("stopnotify")]
    public class StopNotifyModule : ModuleBase<SocketCommandContext>
    {
        private readonly IDatabaseRepository _repo;

        public StopNotifyModule(IDatabaseRepository repo)
        {
            _repo = repo;
        }

        [Command("sunrise")]
        [Summary("Sets a notification time for sunrise")]
        public async Task StopNotifySunriseAsync()
        {
            var channelId = Context.Channel.Id;
            var notification = await _repo.GetNotificationByChannelIdAsync(channelId);

            if (notification == null)
            {
                await ReplyAsync("I don't have a location for you, so I can't notify you of anything anyway.");
                return;
            }

            notification.NotifySunrise = null;
            notification.NextNotifySunriseUtc = null;
            await _repo.SaveNotificationAsync(notification);
            await ReplyAsync("I will no longer notify you of sunrise.");
        }

        [Command("sunset")]
        [Summary("Sets a notification time for sunset")]
        public async Task StopNotifySunsetAsync()
        {
            var channelId = Context.Channel.Id;
            var notification = await _repo.GetNotificationByChannelIdAsync(channelId);

            if (notification == null)
            {
                await ReplyAsync("I don't have a location for you, so I can't notify you of anything anyway.");
                return;
            }

            notification.NotifySunset = null;
            notification.NextNotifySunsetUtc = null;
            await _repo.SaveNotificationAsync(notification);
            await ReplyAsync("I will no longer notify you of sunset.");
        }
    }
}
