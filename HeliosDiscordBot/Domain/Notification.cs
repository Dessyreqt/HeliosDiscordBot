namespace HeliosDiscordBot.Domain
{
    using System;

    public class Notification
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int? NotifySunrise { get; set; }
        public int? NotifySunset { get; set; }
        public DateTime? NextNotifySunrise { get; set; }
        public DateTime? NextNotifySunset { get; set; }
        public string Timezone { get; set; }
    }
}
