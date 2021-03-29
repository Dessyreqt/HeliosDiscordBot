namespace HeliosDiscordBot.Tests.Domain.JulianDate
{
    using System;
    using Shouldly;
    using Xunit;

    public class JulianDateTests
    {
        [Theory]
        [InlineData(2021, 3, 28, 0, 0, 0, 2459301.50000)]
        [InlineData(2010, 1, 9, 23, 15, 0, 2455206.46875)]
        [InlineData(2010, 2, 14, 13, 21, 0, 2455242.05625)]
        [InlineData(2010, 1, 28, 9, 25, 0, 2455224.89236)]
        public void ConvertToJulianDateWorks(int year, int month, int day, int hour, int minute, int second, double julianDate)
        {
            HeliosDiscordBot.Domain.JulianDate.Convert(year, month, day, hour, minute, second).ShouldBe(julianDate, 0.00001);
        }

        [Theory]
        [InlineData(2021, 3, 28, 0, 0, 0, 2459301.50000)]
        [InlineData(2010, 1, 9, 23, 15, 0, 2455206.46875)]
        [InlineData(2010, 2, 14, 13, 21, 0, 2455242.05625)]
        [InlineData(2010, 1, 28, 9, 25, 0, 2455224.89236)]
        public void ConvertFromJulianDateWorks(int year, int month, int day, int hour, int minute, int second, double julianDate)
        {
            var dateTime = HeliosDiscordBot.Domain.JulianDate.Convert(julianDate);

            dateTime.ShouldSatisfyAllConditions(
                () => dateTime.Year.ShouldBe(year),
                () => dateTime.Month.ShouldBe(month),
                () => dateTime.Day.ShouldBe(day),
                () => dateTime.Hour.ShouldBe(hour),
                () => dateTime.Minute.ShouldBe(minute),
                () => dateTime.Second.ShouldBe(second)
            );
        }
    }
}
