using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Services.HourCalculator;
using SchedulerApplication.Services.Interfaces;
using SchedulerApplication.ValueObjects;

namespace SchedulerApp.Testing.HourCalculatorServiceTest;

public class HourCalculatorServiceTests
{
    private readonly IHourCalculatorService _hourCalculatorService = new HourCalculatorService();

    [Fact]
    public void CalculateHour_ShouldThrowArgumentOutOfRangeException_ForInvalidHourlyFrequency()
    {
        // Arrange
        var baseDate = new DateTime(2024, 01, 01);
        var timeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, (DailyHourFrequency)999);

        // Act
        Action act = () => _hourCalculatorService.CalculateHour(baseDate, timeRange);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CalculateHour_ShouldReturnCorrectExecution_ForOnceFrequency()
    {
        // Arrange
        var baseDate = new DateTime(2024, 01, 01);
        var timeRange = new HourTimeRange(new TimeSpan(9, 0, 0));

        // Act
        var result = _hourCalculatorService.CalculateHour(baseDate, timeRange);

        // Assert
        result.Should().ContainSingle().Which.Should().Be(baseDate.Date.Add(timeRange.StartHour));
    }

    [Theory]
    [InlineData("2024-01-01", "09:00:00", "17:00:00", 1, new[] { "2024-01-01 09:00:00", "2024-01-01 10:00:00", "2024-01-01 11:00:00", "2024-01-01 12:00:00", "2024-01-01 13:00:00", "2024-01-01 14:00:00", "2024-01-01 15:00:00", "2024-01-01 16:00:00", "2024-01-01 17:00:00" })]
    [InlineData("2024-01-01", "23:00:00", "02:00:00", 1, new[] { "2024-01-01 23:00:00", "2024-01-02 00:00:00", "2024-01-02 01:00:00", "2024-01-02 02:00:00" })]
    [InlineData("2024-01-01", "10:00:00", "12:00:00", 2, new[] { "2024-01-01 10:00:00", "2024-01-01 12:00:00" })]
    [InlineData("2024-01-01", "09:00:00", "23:00:00", 1, new[] { "2024-01-01 09:00:00", "2024-01-01 10:00:00", "2024-01-01 11:00:00", "2024-01-01 12:00:00", "2024-01-01 13:00:00", "2024-01-01 14:00:00", "2024-01-01 15:00:00", "2024-01-01 16:00:00", "2024-01-01 17:00:00", "2024-01-01 18:00:00", "2024-01-01 19:00:00", "2024-01-01 20:00:00" })]
    public void CalculateHour_ShouldReturnCorrectExecutions_ForRecurrentFrequency(string baseDateString, string startHourString, string endHourString, int intervalHours, string[] expectedTimes)
    {
        // Arrange
        var baseDate = DateTime.Parse(baseDateString);
        var startHour = TimeSpan.Parse(startHourString);
        var endHour = TimeSpan.Parse(endHourString);
        var timeRange = new HourTimeRange(startHour, endHour, intervalHours, DailyHourFrequency.Recurrent);
        var expectedExecutionTimes = expectedTimes.Select(DateTime.Parse).ToList();

        // Act
        var result = _hourCalculatorService.CalculateHour(baseDate, timeRange).Take(12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedExecutionTimes);
    }

    [Fact]
    public void CalculateHour_ShouldHandleCrossMidnight_Correctly()
    {
        // Arrange
        var baseDate = new DateTime(2024, 01, 01);
        var timeRange = new HourTimeRange(new TimeSpan(23, 0, 0), new TimeSpan(2, 0, 0), 1, DailyHourFrequency.Recurrent);
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01, 23, 0, 0),
            new DateTime(2024, 01, 02, 0, 0, 0),
            new DateTime(2024, 01, 02, 1, 0, 0),
            new DateTime(2024, 01, 02, 2, 0, 0)
        };

        // Act
        var result = _hourCalculatorService.CalculateHour(baseDate, timeRange).Take(12).ToList(); 

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void CalculateHour_ShouldHandleSameDay_Correctly()
    {
        // Arrange
        var baseDate = new DateTime(2024, 01, 01);
        var timeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent);
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01, 9, 0, 0),
            new DateTime(2024, 01, 01, 10, 0, 0),
            new DateTime(2024, 01, 01, 11, 0, 0),
            new DateTime(2024, 01, 01, 12, 0, 0)
        };

        // Act
        var result = _hourCalculatorService.CalculateHour(baseDate, timeRange);

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }
}