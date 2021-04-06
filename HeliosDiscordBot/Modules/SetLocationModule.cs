namespace HeliosDiscordBot.Modules
{
    using System.Threading.Tasks;
    using Discord.Commands;
    using HeliosDiscordBot.Domain;
    using HeliosDiscordBot.Repository;

    public class SetLocationModule : ModuleBase<SocketCommandContext>
    {
        private readonly IDatabaseRepository _repo;

        public SetLocationModule(IDatabaseRepository repo)
        {
            _repo = repo;
        }

        [Command("setlocation")]
        [Summary("Sets a location for future sunrise/sunset notifications.")]
        public async Task SetLocationAsync(double latitude, double longitude)
        {
            var channelId = Context.Channel.Id;
            var notification = await _repo.GetNotificationByChannelIdAsync(channelId);
            if (notification == null)
            {
                var user = Context.User;
                notification = new Notification { Username = $"{user.Username}#{user.Discriminator}", ChannelId = channelId.ToString() };
            }

            notification.Latitude = latitude;
            notification.Longitude = longitude;
            notification.NextNotifySunriseUtc = null;
            notification.NextNotifySunsetUtc = null;

            await _repo.SaveNotificationAsync(notification);
            await ReplyAsync($"Your location is now latitude {notification.Latitude:###.#######}, longitude {notification.Longitude:###.#######}.");
        }
    }
}
