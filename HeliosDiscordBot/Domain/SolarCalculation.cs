namespace HeliosDiscordBot.Domain
{
    using System;
    using static System.Math;

    public class SolarCalculation
    {
        private double _latitude;
        private double _longitude;
        private double _julianDate;
        private double _julianDay;
        private double _meanSolarTime;
        private double _solarMeanAnomaly;
        private double _solarMeanAnomalyRadians;
        private double _center;

        public SolarCalculation(double latitude, double longitude, DateTime dateTime)
        {
            _latitude = latitude;
            _longitude = longitude;
            _julianDate = JulianDate.Convert(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            _julianDay = _julianDate - 2451545 + 0.0008d;
            _meanSolarTime = _julianDay - (longitude / 360);
            _solarMeanAnomaly = (357.5291d + 0.98560028d * _meanSolarTime) % 360;
            _solarMeanAnomalyRadians = PI * _solarMeanAnomaly / 180d;
            _center = 1.9148 * Sin(_solarMeanAnomalyRadians) + 0.0200 * Sin(2 * _solarMeanAnomalyRadians) + 0.0003 * Sin(3 * _solarMeanAnomalyRadians);

        }
    }
}
