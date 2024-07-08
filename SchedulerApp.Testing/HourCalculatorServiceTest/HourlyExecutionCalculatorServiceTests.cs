using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.HourCalculator;
using SchedulerApplication.Services.Interfaces;
using SchedulerApplication.ValueObjects;

namespace SchedulerApp.Testing.HourCalculatorServiceTest;

public class HourlyExecutionCalculatorServiceTests
{
    private readonly IHourlyExecutionCalculatorService _hourCalculatorService;

    public HourlyExecutionCalculatorServiceTests()
    {
        _hourCalculatorService = new HourlyExecutionCalculatorService();
    }

    [Fact]
    public void CalculateHourlyExecutions_ShouldHandleRecurrentFrequency()
    {
        // Arrange
        var config = new RecurringSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
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
            new DateTime(2024, 01, 01, 17, 0, 0)
        };

        // Act
        var result = _hourCalculatorService.CalculateHourlyExecutions(config).Take(9).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void CalculateHourlyExecutions_ShouldHandleCrossMidnight()
    {
        // Arrange
        var config = new RecurringSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(new TimeSpan(23, 0, 0), new TimeSpan(2, 0, 0), 1, DailyHourFrequency.Recurrent)
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01, 23, 0, 0),
            new DateTime(2024, 01, 02, 0, 0, 0),
            new DateTime(2024, 01, 02, 1, 0, 0),
            new DateTime(2024, 01, 02, 2, 0, 0)
        };

        // Act
        var result = _hourCalculatorService.CalculateHourlyExecutions(config).Take(12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void CalculateHourlyExecutions_ShouldHandleExact12Executions()
    {
        // Arrange
        var config = new RecurringSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(new TimeSpan(0, 0, 0), new TimeSpan(11, 0, 0), 1, DailyHourFrequency.Recurrent)
        };
        var expectedTimes = Enumerable.Range(0, 12).Select(i => new DateTime(2024, 01, 01).AddHours(i)).ToList();

        // Act
        var result = _hourCalculatorService.CalculateHourlyExecutions(config).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void CalculateHourlyExecutions_ShouldHandleCrossMidnight_WithLargeInterval()
    {
        // Arrange
        var config = new RecurringSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(new TimeSpan(22, 0, 0), new TimeSpan(2, 0, 0), 3, DailyHourFrequency.Recurrent)
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01, 22, 0, 0),
            new DateTime(2024, 01, 02, 1, 0, 0),
        };

        // Act
        var result = _hourCalculatorService.CalculateHourlyExecutions(config).Take(12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void CalculateHourlyExecutions_ShouldHandleCrossMultipleDays()
    {
        // Arrange
        var config = new RecurringSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(new TimeSpan(20, 0, 0), new TimeSpan(6, 0, 0), 4, DailyHourFrequency.Recurrent)
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01, 20, 0, 0),
            new DateTime(2024, 01, 02, 0, 0, 0),
            new DateTime(2024, 01, 02, 4, 0, 0),
        };

        // Act
        var result = _hourCalculatorService.CalculateHourlyExecutions(config).Take(12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }


    [Fact]
    public void CalculateHourlyExecutions_ShouldHandleRecurrentFrequency_WithEndHourBeforeStartHour()
    {
        // Arrange
        var config = new RecurringSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(new TimeSpan(22, 0, 0), new TimeSpan(2, 0, 0), 2, DailyHourFrequency.Recurrent)
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01, 22, 0, 0),
            new DateTime(2024, 01, 02, 0, 0, 0),
            new DateTime(2024, 01, 02, 2, 0, 0)
        };

        // Act
        var result = _hourCalculatorService.CalculateHourlyExecutions(config).Take(12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }
}
