namespace HeliosDiscordBot.Domain
{
    using System;
    using System.Linq;

    public static class TimeZoneConverter
    {
        public static DateTime ToTimeZone(this DateTime currentDate, string notificationTimezone)
        {
            var foundZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(zone => zone.DisplayName == notificationTimezone);

            if (foundZone == null)
            {
                return currentDate;
            }

            var currentTimeInZone = TimeZoneInfo.ConvertTimeFromUtc(currentDate, foundZone);

            return currentTimeInZone;
        }
    }
}
