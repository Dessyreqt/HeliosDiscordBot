namespace HeliosDiscordBot.Tests.Domain.SolarCalculationTests
{
    using System;
    using HeliosDiscordBot.Domain;
    using Shouldly;
    using Xunit;

    public class JulianDateTests
    {
        [Theory]
        [InlineData(2021, 3, 28, 12, 4, 58, 5, 43, 34, 18, 26, 22)]
        [InlineData(2010, 1, 9, 12, 7, 6, 8, 2, 58, 16, 11, 14)]
        [InlineData(2010, 2, 14, 12, 14, 12, 7, 15, 41, 17, 12, 42)]
        [InlineData(2010, 1, 28, 12, 12, 57, 7, 44, 5, 16, 41, 50)]
        [InlineData(2021, 11, 9, 11, 43, 48, 7, 8, 6, 16, 19, 29)]
        public void CalculateSolarEventsCorrectly(
            int year,
            int month,
            int day,
            int solarTransitHour,
            int solarTransitMinute,
            int solarTransitSecond,
            int sunriseHour,
            int sunriseMinute,
            int sunriseSecond,
            int sunsetHour,
            int sunsetMinute,
            int sunsetSecond)
        {
            var latitude = 51.48;
            var longitude = 0;
            var dateTime = new DateTime(year, month, day);

            var solarCalculation = new SolarCalculation(latitude, longitude, dateTime);

            solarCalculation.ShouldSatisfyAllConditions(
                () => solarCalculation.SolarTransit.Year.ShouldBe(year),
                () => solarCalculation.SolarTransit.Month.ShouldBe(month),
                () => solarCalculation.SolarTransit.Day.ShouldBe(day),
                () => solarCalculation.SolarTransit.Hour.ShouldBe(solarTransitHour),
                () => solarCalculation.SolarTransit.Minute.ShouldBe(solarTransitMinute),
                () => solarCalculation.SolarTransit.Second.ShouldBe(solarTransitSecond),
                () => solarCalculation.Sunrise.Year.ShouldBe(year),
                () => solarCalculation.Sunrise.Month.ShouldBe(month),
                () => solarCalculation.Sunrise.Day.ShouldBe(day),
                () => solarCalculation.Sunrise.Hour.ShouldBe(sunriseHour),
                () => solarCalculation.Sunrise.Minute.ShouldBe(sunriseMinute),
                () => solarCalculation.Sunrise.Second.ShouldBe(sunriseSecond),
                () => solarCalculation.Sunset.Year.ShouldBe(year),
                () => solarCalculation.Sunset.Month.ShouldBe(month),
                () => solarCalculation.Sunset.Day.ShouldBe(day),
                () => solarCalculation.Sunset.Hour.ShouldBe(sunsetHour),
                () => solarCalculation.Sunset.Minute.ShouldBe(sunsetMinute),
                () => solarCalculation.Sunset.Second.ShouldBe(sunsetSecond)
            );
        }
    }
}
