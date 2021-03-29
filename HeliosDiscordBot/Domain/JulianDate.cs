namespace HeliosDiscordBot.Domain
{
    using System;

    public static class JulianDate
    {
        private static DateTime _julianPeriodStart = new DateTime(-4713, 01, 01);

        public static double Convert(DateTime dateTime) => Convert(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);

        public static double Convert(int year, int month, int day, int hour, int minute, int second)
        {
            // (1461 * (Y + 4800 + (M - 14)/12))/4 +(367 * (M - 2 - 12 * ((M - 14)/12)))/12 - (3 * ((Y + 4900 + (M - 14)/12)/100))/4 + D - 32075 
            var julianDay = (1461 * (year + 4800 + (month - 14) / 12)) / 4 + (367 * (month - 2 - 12 * ((month - 14)/ 12))) / 12 - (3 * ((year + 4900 + (month - 14) / 12) / 100)) / 4 + day - 32075;
            var julianTime = (hour - 12) / 24d + minute / 1440d + second / 86400d;
            
            return julianDay + julianTime;
        }

        public static DateTime Convert(double julianDate)
        {
            // Richards algorithm from wikipedia
            var y = 4716;
            var j = 1401;
            var m = 2;
            var n = 12;
            var r = 4;
            var p = 1461;
            var v = 3;
            var u = 5;
            var s = 153;
            var w = 2;
            var B = 274277;
            var C = -38;
            var J = (int)julianDate;
            var f = J + j + (((4 * J + B) / 146097) * 3) / 4 + C;
            var e = r * f + v;
            var g = (e % p) / r;
            var h = u * g + w;
            var D = (h % s) / u + 1;
            var M = ((h / s + m) % n) + 1;
            var Y = (e / p) - y + (n + m - M) / n;

            var julianTime = julianDate - J;
            var addHour = julianTime * 24;
            var addHourInt = (int)addHour;
            var addMinute = (addHour - addHourInt) * 60;
            var addMinuteInt = (int)addMinute;
            var addSecond = (addMinute - addMinuteInt) * 60;
            var addSecondInt = Math.Round(addSecond, MidpointRounding.AwayFromZero);

            var dateTime = new DateTime(Y, M, D, 12, 0, 0, DateTimeKind.Utc).AddHours(addHourInt).AddMinutes(addMinuteInt).AddSeconds(addSecondInt);
            return dateTime;
        }
    }
}
