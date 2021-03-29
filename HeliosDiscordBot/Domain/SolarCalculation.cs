namespace HeliosDiscordBot.Domain
{
    using System;
    using static System.Math;

    public class SolarCalculation
    {
        private readonly double _solarTransitJulianDate;
        private readonly double _sunriseJulianDate;
        private readonly double _sunsetJulianDate;

        public SolarCalculation(double latitude, double longitude, DateTime dateTime)
        {
            var date = dateTime.Date.AddHours(12);
            var julianDate = JulianDate.Convert(date);
            var julianDay = julianDate - 2451545 + 0.0008d;
            var julianCentury = julianDay / 36525; // julianCentury
            var geometricMeanSolarLongitudeDegrees = (280.46646 + julianCentury * (36000.76983 + julianCentury * 0.0003032)) % 360; // geometricMeanSolarLongitudeDegrees
            var geometricMeanSolarAnomalyDegrees = 357.52911 + julianCentury * (35999.05029 - 0.0001537 * julianCentury); // geometricMeanSolarAnomalyDegrees
            var earthOrbitEccentricity = 0.016708634 - julianCentury * (0.000042037 + 0.0000001267 * julianCentury); // earthOrbitEccentricity
            var solarCenter = Sin(Radians(geometricMeanSolarAnomalyDegrees)) * (1.914602 - julianCentury * (0.004817 + 0.000014 * julianCentury)) + Sin(Radians(2 * geometricMeanSolarAnomalyDegrees)) * (0.019993 - 0.000101 * julianCentury) + Sin(Radians(3 * geometricMeanSolarAnomalyDegrees)) * 0.000289; // solarCenter
            var trueSolarLongitudeDegrees = geometricMeanSolarLongitudeDegrees + solarCenter; // trueSolarLongitudeDegrees
            var apparentSolarLongitudeDegrees = trueSolarLongitudeDegrees - 0.00569 - 0.00478 * Sin(Radians(125.04 - 1934.136 * julianCentury)); // apparentSolarLongitudeDegrees
            var meanObliqueEclipticDegrees = 23 + (26 + ((21.448 - julianCentury * (46.815 + julianCentury * (0.00059 - julianCentury * 0.001813)))) / 60) / 60; // meanObliqueEclipticDegrees
            var correctedObliqueEclipticDegrees = meanObliqueEclipticDegrees + 0.00256 * Cos(Radians(125.04 - 1934.136 * julianCentury)); // obliqueCorrected
            var solarDeclinationDegrees = Degrees(Asin(Sin(Radians(correctedObliqueEclipticDegrees)) * Sin(Radians(apparentSolarLongitudeDegrees)))); // solarDeclinationDegrees
            var y = Tan(Radians(correctedObliqueEclipticDegrees / 2)) * Tan(Radians(correctedObliqueEclipticDegrees / 2)); // y
            var timeAdjustment = 4 * Degrees(
                          y * Sin(2 * Radians(geometricMeanSolarLongitudeDegrees)) - 2 * earthOrbitEccentricity * Sin(Radians(geometricMeanSolarAnomalyDegrees)) + 4 * earthOrbitEccentricity * y * Sin(Radians(geometricMeanSolarAnomalyDegrees)) * Cos(2 * Radians(geometricMeanSolarLongitudeDegrees)) -
                          0.5 * y * y * Sin(4 * Radians(geometricMeanSolarLongitudeDegrees)) - 1.25 * earthOrbitEccentricity * earthOrbitEccentricity * Sin(2 * Radians(geometricMeanSolarAnomalyDegrees))); // timeAdjustment
            var hourAngleDegrees = Degrees(Acos(Cos(Radians(90.833)) / (Cos(Radians(latitude)) * Cos(Radians(solarDeclinationDegrees))) - Tan(Radians(latitude)) * Tan(Radians(solarDeclinationDegrees)))); // hourAngleDegrees

            _solarTransitJulianDate = julianDate - (4 * longitude + timeAdjustment) / 1440;
            _sunriseJulianDate = _solarTransitJulianDate - hourAngleDegrees / 360;
            _sunsetJulianDate = _solarTransitJulianDate + hourAngleDegrees / 360;
        }

        public DateTime SolarTransit => JulianDate.Convert(_solarTransitJulianDate);
        public DateTime Sunrise => JulianDate.Convert(_sunriseJulianDate);
        public DateTime Sunset => JulianDate.Convert(_sunsetJulianDate);

        private double Degrees(double radians) => radians * 180 / PI;

        private double Radians(double degrees) => degrees * PI / 180;
    }
}
