namespace HeliosDiscordBot.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using HeliosDiscordBot.Domain;
    using HeliosDiscordBot.Settings;
    using Microsoft.Extensions.Options;

    public class DatabaseRepository : IDatabaseRepository
    {
        private readonly DatabaseSettings _databaseSettings;

        public DatabaseRepository(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseSettings = databaseSettings.Value;
        }

        public async Task<Notification> GetNotificationByChannelIdAsync(ulong channelId)
        {
            var channelIdString = channelId.ToString();
            var sql = "SELECT * FROM [dbo].[Notification] WHERE [ChannelId] = @channelId";
            using var connection = new SqlConnection(_databaseSettings.ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<Notification>(sql, new { channelId = channelIdString });
        }

        public async Task SaveNotificationAsync(Notification notification)
        {
            string sql;

            if (notification.NotificationId == 0)
            {
                sql =
                    "INSERT INTO [dbo].[Notification] (Username, ChannelId, Latitude, Longitude, NotifySunrise, NotifySunset, NextNotifySunriseUtc, NextNotifySunsetUtc, Timezone) VALUES (@Username, @ChannelId, @Latitude, @Longitude, @NotifySunrise, @NotifySunset, @NextNotifySunriseUtc, @NextNotifySunsetUtc, @Timezone)";
            }
            else
            {
                sql =
                    "UPDATE [dbo].[Notification] SET Username = @Username, ChannelId = @ChannelId, Latitude = @Latitude, Longitude = @Longitude, NotifySunrise = @NotifySunrise, NotifySunset = @NotifySunset, NextNotifySunriseUtc = @NextNotifySunriseUtc, NextNotifySunsetUtc = @NextNotifySunsetUtc, Timezone = @Timezone WHERE [NotificationId] = @NotificationId";
            }

            using var connection = new SqlConnection(_databaseSettings.ConnectionString);
            await connection.ExecuteAsync(sql, notification);
        }

        public async Task<List<Notification>> GetUnsetNotificationsAsync()
        {
            var sql = "SELECT * FROM [Notification] WHERE ([NotifySunrise] IS NOT NULL AND [NextNotifySunriseUtc] IS NULL) OR ([NotifySunset] IS NOT NULL AND [NextNotifySunsetUtc] IS NULL)";
            using var connection = new SqlConnection(_databaseSettings.ConnectionString);
            return (await connection.QueryAsync<Notification>(sql)).ToList();
        }

        public async Task<List<Notification>> GetUnsetNotificationsAsync(DateTime currentTime)
        {
            var sql = "SELECT * FROM [Notification] WHERE ([NotifySunrise] IS NOT NULL AND [NextNotifySunriseUtc] < @currentTime) OR ([NotifySunset] IS NOT NULL AND [NextNotifySunsetUtc] < @currentTime)";
            using var connection = new SqlConnection(_databaseSettings.ConnectionString);
            return (await connection.QueryAsync<Notification>(sql)).ToList();
        }
    }
}
