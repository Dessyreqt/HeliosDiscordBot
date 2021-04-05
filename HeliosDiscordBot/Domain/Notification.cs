namespace HeliosDiscordBot.Domain
{
    using System;

    public class Notification
    {
        public int NotificationId { get; set; }
        public string Username { get; set; }
        public string ChannelId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int? NotifySunrise { get; set; }
        public int? NotifySunset { get; set; }
        public DateTime? NextNotifySunriseUtc { get; set; }
        public DateTime? NextNotifySunsetUtc { get; set; }
        public string Timezone { get; set; }
    }
}
