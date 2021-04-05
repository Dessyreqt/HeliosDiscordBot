namespace HeliosDiscordBot.Repository
{
    using System.Threading.Tasks;
    using HeliosDiscordBot.Domain;

    public interface IDatabaseRepository
    {
        Task<Notification> GetNotificationByChannelIdAsync(ulong channelId);
        Task SaveNotificationAsync(Notification notification);
    }
}
