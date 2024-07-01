using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Services.HourCalculator;
using SchedulerApplication.ValueObjects;

namespace SchedulerApp.Testing;

    public class HourCalculatorServiceTests
    {
    [Theory]
    [InlineData("2024-06-25", "09:00", "17:00", 2, DailyHourFrequency.Recurrent,
    new[] { "2024-06-25 09:00", "2024-06-25 11:00", "2024-06-25 13:00", "2024-06-25 15:00", "2024-06-25 17:00" })]
    [InlineData("2024-06-25", "08:00", "14:00", 3, DailyHourFrequency.Recurrent,
    new[] { "2024-06-25 08:00", "2024-06-25 11:00", "2024-06-25 14:00" })]
    public void CalculateHour_ShouldReturnCorrectHours_WhenRecurrent(
    string date, string startHour, string endHour, int hourlyInterval, DailyHourFrequency frequency, string[] expectedHours)
    {
        // Arrange
        var hourCalculator = new HourCalculatorService();
        var timeRange = new HourTimeRange(TimeSpan.Parse(startHour), TimeSpan.Parse(endHour), hourlyInterval, frequency);
        var dateTime = DateTime.Parse(date);
        var expectedDateTimes = expectedHours.Select(DateTime.Parse).ToList();

        // Act
        var result = hourCalculator.CalculateHour(dateTime, timeRange);

        // Assert
        result.Should().BeEquivalentTo(expectedDateTimes);
    }

    [Fact]
    public void CalculateHour_ShouldReturnSingleHour_WhenOnce()
    {
        // Arrange
        var hourCalculator = new HourCalculatorService();
        var timeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 2, DailyHourFrequency.Once);
        var dateTime = new DateTime(2024, 06, 25);
        var expectedDateTimes = new List<DateTime> { new DateTime(2024, 06, 25, 9, 0, 0) };

        // Act
        var result = hourCalculator.CalculateHour(dateTime, timeRange);

        // Assert
        result.Should().BeEquivalentTo(expectedDateTimes);
    }

    [Fact]
    public void CalculateHour_ShouldThrowException_WhenStartHourGreaterThanEndHour()
    {
        // Arrange
        var hourCalculator = new HourCalculatorService();

        // Act
        var act = () =>
        {
            var hourTimeRange = new HourTimeRange(new TimeSpan(17, 0, 0), new TimeSpan(9, 0, 0), 2,
                DailyHourFrequency.Recurrent);
        };

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("StartHour must be less than or equal to EndHour.");
    }

    [Fact]
    public void CalculateHour_ShouldThrowException_WhenHourlyIntervalIsNonPositive()
    {
        // Arrange
        var hourCalculator = new HourCalculatorService();

        // Act
        Action act = () => new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 0, DailyHourFrequency.Recurrent);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("HourlyInterval must be greater than 0.");
    }
}

