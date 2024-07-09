using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Services.HourCalculator;
using SchedulerApplication.Services.Interfaces;
using SchedulerApplication.Services.WeekCalculator;
using SchedulerApplication.ValueObjects;

namespace SchedulerApp.Testing.WeekCalculator;

public class WeeklyExecutionCalculatorServiceTests
{
    private readonly IHourlyExecutionCalculatorService _hourlyExecutionCalculatorService;
    private readonly WeeklyExecutionCalculatorService _weeklyExecutionCalculatorService;

    public WeeklyExecutionCalculatorServiceTests()
    {
        _hourlyExecutionCalculatorService = new HourlyExecutionCalculatorService();
        _weeklyExecutionCalculatorService = new WeeklyExecutionCalculatorService(_hourlyExecutionCalculatorService);
    }

    [Fact]
    public void CalculateWeeklyExecutions_ShouldReturnCorrectExecutions_ForSingleDay()
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday },
            WeekInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent),
            Limits = new LimitsTimeInterval(new DateTime(2024,01,01), new DateTime(2024, 01, 03))
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01, 9, 0, 0),
            new DateTime(2024, 01, 01, 10, 0, 0),
            new DateTime(2024, 01, 01, 11, 0, 0),
            new DateTime(2024, 01, 01, 12, 0, 0),
            new DateTime(2024, 01, 01, 13, 0, 0),
            new DateTime(2024, 01, 01, 14, 0, 0),
            new DateTime(2024, 01, 01, 15, 0, 0),
            new DateTime(2024, 01, 01, 16, 0, 0),
            new DateTime(2024, 01, 01, 17, 0, 0)
        };

        // Act
        var result = _weeklyExecutionCalculatorService.CalculateWeeklyExecutions(config, 12).Take(9).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void CalculateWeeklyExecutions_ShouldHandleMultipleDaysOfWeek()
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
            WeekInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01, 9, 0, 0),
            new DateTime(2024, 01, 01, 10, 0, 0),
            new DateTime(2024, 01, 01, 11, 0, 0),
            new DateTime(2024, 01, 01, 12, 0, 0),
            new DateTime(2024, 01, 01, 13, 0, 0),
            new DateTime(2024, 01, 01, 14, 0, 0),
            new DateTime(2024, 01, 01, 15, 0, 0),
            new DateTime(2024, 01, 01, 16, 0, 0),
            new DateTime(2024, 01, 01, 17, 0, 0),
            new DateTime(2024, 01, 03, 9, 0, 0),
            new DateTime(2024, 01, 03, 10, 0, 0),
            new DateTime(2024, 01, 03, 11, 0, 0)
        };

        // Act
        var result = _weeklyExecutionCalculatorService.CalculateWeeklyExecutions(config, 12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void CalculateWeeklyExecutions_ShouldReturnCorrectExecutions_ForWeekInterval()
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday },
            WeekInterval = 2,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01, 9, 0, 0),
            new DateTime(2024, 01, 01, 10, 0, 0),
            new DateTime(2024, 01, 01, 11, 0, 0),
            new DateTime(2024, 01, 01, 12, 0, 0),
            new DateTime(2024, 01, 01, 13, 0, 0),
            new DateTime(2024, 01, 01, 14, 0, 0),
            new DateTime(2024, 01, 01, 15, 0, 0),
            new DateTime(2024, 01, 01, 16, 0, 0),
            new DateTime(2024, 01, 01, 17, 0, 0),
            new DateTime(2024, 01, 15, 9, 0, 0),
            new DateTime(2024, 01, 15, 10, 0, 0),
            new DateTime(2024, 01, 15, 11, 0, 0)
        };

        // Act
        var result = _weeklyExecutionCalculatorService.CalculateWeeklyExecutions(config, 12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }
    [Fact]
    public void CalculateWeeklyExecutions_ShouldHandleCrossingYearBoundary()
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2023, 12, 30),
            IsEnabled = true,
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Saturday },
            WeekInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2023, 12, 30, 9, 0, 0),
            new DateTime(2023, 12, 30, 10, 0, 0),
            new DateTime(2023, 12, 30, 11, 0, 0),
            new DateTime(2023, 12, 30, 12, 0, 0),
            new DateTime(2023, 12, 30, 13, 0, 0),
            new DateTime(2023, 12, 30, 14, 0, 0),
            new DateTime(2023, 12, 30, 15, 0, 0),
            new DateTime(2023, 12, 30, 16, 0, 0),
            new DateTime(2023, 12, 30, 17, 0, 0),
            new DateTime(2024, 01, 06, 9, 0, 0),
            new DateTime(2024, 01, 06, 10, 0, 0),
            new DateTime(2024, 01, 06, 11, 0, 0)
        };

        // Act
        var result = _weeklyExecutionCalculatorService.CalculateWeeklyExecutions(config, 12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

}