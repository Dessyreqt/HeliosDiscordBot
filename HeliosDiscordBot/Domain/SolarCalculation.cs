namespace HeliosDiscordBot.Domain
{
    using System;
    using static System.Math;

    public class SolarCalculation
    {
        private double _latitude;
        private double _longitude;
        private double _latitudeRadians;
        private double _julianDate;
        private double _julianDay;
        private double _meanSolarTime;
        private double _solarMeanAnomalyDegrees;
        private double _solarMeanAnomalyRadians;
        private double _center;
        private double _eclipticLongitudeDegrees;
        private double _eclipticLongitudeRadians;
        private double _solarTransitJulianDate;
        private double _axialTiltRadians;
        private double _declinationOfSunRadians;
        private double _altitudeSolarCenterRadians;
        private double _hourAngleRadians;
        private double _sunriseJulianDate;
        private double _sunsetJulianDate;

        public SolarCalculation(double latitude, double longitude, DateTime dateTime)
        {
            _latitude = latitude;
            _longitude = longitude;
            _latitudeRadians = PI * _latitude / 180d;
            _julianDate = JulianDate.Convert(dateTime);
            _julianDay = _julianDate - 2451545 + 0.0008d;
            _meanSolarTime = _julianDay - (_longitude / 360);
            _solarMeanAnomalyDegrees = (357.5291d + 0.98560028d * _meanSolarTime) % 360;
            _solarMeanAnomalyRadians = PI * _solarMeanAnomalyDegrees / 180d;
            _center = 1.9148 * Sin(_solarMeanAnomalyRadians) + 0.02 * Sin(2 * _solarMeanAnomalyRadians) + 0.0003 * Sin(3 * _solarMeanAnomalyRadians);
            _eclipticLongitudeDegrees = (_solarMeanAnomalyDegrees + _center + 180 + 102.9372) % 360d;
            _eclipticLongitudeRadians = PI * _eclipticLongitudeDegrees / 180d;
            _solarTransitJulianDate = 2451545d + _meanSolarTime + 0.0053 * Sin(_solarMeanAnomalyRadians) - 0.0069 * Sin(2 * _eclipticLongitudeRadians);
            _axialTiltRadians = 23.44 / 180d;
            _declinationOfSunRadians = Asin(Sin(_eclipticLongitudeRadians) * Sin(_axialTiltRadians));
            _altitudeSolarCenterRadians = PI * -0.83 / 180d;
            _hourAngleRadians = Acos((Sin(_altitudeSolarCenterRadians) - Sin(_latitudeRadians) * Sin(_declinationOfSunRadians)) / (Cos(_latitudeRadians) * Cos(_declinationOfSunRadians)));
            _sunriseJulianDate = _solarTransitJulianDate - _hourAngleRadians / (2 * PI);
            _sunsetJulianDate = _solarTransitJulianDate + _hourAngleRadians / (2 * PI);
        }
    }
}
