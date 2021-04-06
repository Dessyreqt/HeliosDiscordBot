namespace HeliosDiscordBot.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HeliosDiscordBot.Domain;

    public interface IDatabaseRepository
    {
        Task<Notification> GetNotificationByChannelIdAsync(ulong channelId);
        Task SaveNotificationAsync(Notification notification);
        Task<List<Notification>> GetUnsetNotificationsAsync();
        Task<List<Notification>> GetExpiredNotificationsAsync(DateTime currentDate);
    }
}
