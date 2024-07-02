using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Services.HourCalculator;
using SchedulerApplication.ValueObjects;

namespace SchedulerApp.Testing.HourCalculatorServiceTest;

public class HourCalculatorServiceTests
{
    [Theory]
    [InlineData("2024-02-12T20:12:24")]
    [InlineData("2003-06-12")]
    [InlineData("2004-01-12T00:12:24")]

    public void CalculateHour_ShouldReturnHourList_IfDailyHourFrequencyIsOnce(string dateTimeString)
    {

        //Arrange
        var dateTime = DateTime.Parse(dateTimeString);
        var timeRange = new HourTimeRange(dateTime.TimeOfDay);
        var service = new HourCalculatorService();

        var expectedList = new List<DateTime>()
        {
            dateTime.Date.Add(timeRange.StartHour)
        };

        //Act
        var result = service.CalculateHour(dateTime, timeRange);

        //Assert
        result.Should().BeEquivalentTo(expectedList);
    }

    [Fact]
    public void CalculateHour_ShouldReturnException_IfDailyHourFrequencyIsOnceAndStartHourItNull()
    {
        //Arrange
        var dateTime = new DateTime();
        var timeRange = new HourTimeRange(dateTime.TimeOfDay);
        var service = new HourCalculatorService();

        var expectedList = new List<DateTime>()
        {
            dateTime.Date.Add(timeRange.StartHour)
        };

        //Act
        var result = service.CalculateHour(dateTime, timeRange);

        //Assert
        result.Should().BeEquivalentTo(expectedList);

    }
    [Fact]
    public void CalculateHour_ShouldThrowArgumentOutOfRangeException_ForUnsupportedHourlyFrequency()
    {
        // Arrange
        var referenceDate = DateTime.Now;
        var startHour = new TimeSpan(10, 0, 0);
        var endHour = new TimeSpan(12, 0, 0);
        var unsupportedFrequency = (DailyHourFrequency)999;
        var timeRange = new HourTimeRange(startHour, endHour, 1, unsupportedFrequency);
        var hourCalculatorService = new HourCalculatorService();

        // Act
        Action act = () => hourCalculatorService.CalculateHour(referenceDate, timeRange);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}

