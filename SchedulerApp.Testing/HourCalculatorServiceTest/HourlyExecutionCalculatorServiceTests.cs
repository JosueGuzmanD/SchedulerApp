using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Models;
using SchedulerApplication.Models.FrequencyConfigurations;
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
    public void CalculateHourlyExecutions_ShouldHandleOnceFrequency()
    {
        // Arrange
        var config = new OnceSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            ConfigurationDateTime = new DateTime(2024, 01, 01, 9, 0, 0)
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01, 9, 0, 0),
            new DateTime(2024, 01, 02, 9, 0, 0),
            new DateTime(2024, 01, 03, 9, 0, 0),
            new DateTime(2024, 01, 04, 9, 0, 0),
            new DateTime(2024, 01, 05, 9, 0, 0),
            new DateTime(2024, 01, 06, 9, 0, 0),
            new DateTime(2024, 01, 07, 9, 0, 0),
            new DateTime(2024, 01, 08, 9, 0, 0),
            new DateTime(2024, 01, 09, 9, 0, 0),
            new DateTime(2024, 01, 10, 9, 0, 0),
            new DateTime(2024, 01, 11, 9, 0, 0),
            new DateTime(2024, 01, 12, 9, 0, 0)
        };

        // Act
        var result = _hourCalculatorService.CalculateHourlyExecutions(config).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void CalculateHourlyExecutions_ShouldHandleRecurrentDailyFrequency()
    {
        // Arrange
        var config = new DailyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent),
            Limits = new LimitsTimeInterval(new DateTime(2024,01,01), new DateTime(2024,01,03))
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
            new DateTime(2024, 01, 02, 9, 0, 0),
            new DateTime(2024, 01, 02, 10, 0, 0),
            new DateTime(2024, 01, 02, 11, 0, 0)
        };

        // Act
        var result = _hourCalculatorService.CalculateHourlyExecutions(config).Take(12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void CalculateHourlyExecutions_ShouldHandleCrossMidnight()
    {
        // Arrange
        var config = new DailyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(new TimeSpan(23, 0, 0), new TimeSpan(2, 0, 0), 1, DailyHourFrequency.Recurrent),
            Limits = new LimitsTimeInterval(new DateTime(2024,01,01),new DateTime(2024,01,02,2,00,00))
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01, 23, 0, 0),
            new DateTime(2024, 01, 02, 0, 0, 0),
            new DateTime(2024, 01, 02, 1, 0, 0),
            new DateTime(2024, 01, 02, 2, 0, 0)
        };

        // Act
        var result = _hourCalculatorService.CalculateHourlyExecutions(config).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void CalculateHourlyExecutions_ShouldHandleRecurrentWeeklyFrequency()
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent),
            WeekInterval = 1,
            Limits = new LimitsTimeInterval(new DateTime(2024,01,01), new DateTime(2024,01,04)),
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday }
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
        var result = _hourCalculatorService.CalculateHourlyExecutions(config).Take(12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }
    [Fact]
    public void CalculateHourlyExecutions_ShouldHandleWeeklyInterval()
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent),
            WeekInterval = 2,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 16)),
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday }
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
        var result = _hourCalculatorService.CalculateHourlyExecutions(config).Take(12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void CalculateHourlyExecutions_ShouldReturnEmptyForDisabledConfiguration()
    {
        // Arrange
        var config = new OnceSchedulerConfiguration
        {
            IsEnabled = false,
            CurrentDate = new DateTime(2024, 01, 01),
            ConfigurationDateTime = new DateTime(2024, 01, 01, 9, 0, 0)
        };

        // Act
        var result = _hourCalculatorService.CalculateHourlyExecutions(config);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void CalculateHourlyExecutions_ShouldThrowException_ForUnsupportedConfiguration()
    {
        // Arrange
        var unsupportedConfig = new UnsupportedSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01)
        };

        // Act
        Action act = () => _hourCalculatorService.CalculateHourlyExecutions(unsupportedConfig);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Unknown configuration type");
    }

    [Fact]
    public void CalculateHourlyExecutions_ShouldOnlyIncludeHoursWithinLimits()
    {
        // Arrange
        var config = new DailyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01, 10, 0, 0), new DateTime(2024, 01, 01, 15, 0, 0))
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01, 10, 0, 0),
            new DateTime(2024, 01, 01, 11, 0, 0),
            new DateTime(2024, 01, 01, 12, 0, 0),
            new DateTime(2024, 01, 01, 13, 0, 0),
            new DateTime(2024, 01, 01, 14, 0, 0),
            new DateTime(2024, 01, 01, 15, 0, 0)
        };

        // Act
        var result = _hourCalculatorService.CalculateHourlyExecutions(config).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }
    private class UnsupportedSchedulerConfiguration : SchedulerConfiguration
    {
    }

}
