namespace HeliosDiscordBot.Domain
{
    using System;

    public static class JulianDate
    {
        private static DateTime _julianPeriodStart = new DateTime(-4713, 01, 01);

        public static double Convert(int year, int month, int day, int hour, int minute, int second)
        {
            // (1461 * (Y + 4800 + (M - 14)/12))/4 +(367 * (M - 2 - 12 * ((M - 14)/12)))/12 - (3 * ((Y + 4900 + (M - 14)/12)/100))/4 + D - 32075 
            var julianDay = (1461 * (year + 4800 + (month - 14) / 12)) / 4 + (367 * (month - 2 - 12 * ((month - 14)/ 12))) / 12 - (3 * ((year + 4900 + (month - 14) / 12) / 100)) / 4 + day - 32075;
            var julianTime = (hour - 12) / 24d + minute / 1440d + second / 86400d;
            
            return julianDay + julianTime;
        }
    }
}
