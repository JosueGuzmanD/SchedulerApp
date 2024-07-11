using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Services.ExecutionCalculator;
using SchedulerApplication.Services.Interfaces;
using SchedulerApplication.ValueObjects;

namespace SchedulerApp.Testing.WeekCalculator;

public class WeeklyExecutionCalculatorServiceTests
{
    private readonly IWeeklyExecutionCalculatorService _weeklyCalculatorService;

    public WeeklyExecutionCalculatorServiceTests()
    {
        var dailyExecutionCalculatorService = new DailyExecutionCalculatorService();
        _weeklyCalculatorService = new WeeklyExecutionCalculatorService(dailyExecutionCalculatorService);
    }

    [Fact]
    public void CalculateWeeklyExecutions_ShouldHandleWeeklyInterval()
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            WeekInterval = 2,
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 31)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent)
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01, 9, 0, 0),
            new DateTime(2024, 01, 01, 10, 0, 0),
            new DateTime(2024, 01, 01, 11, 0, 0),
            new DateTime(2024, 01, 01, 12, 0, 0),
            new DateTime(2024, 01, 03, 9, 0, 0),
            new DateTime(2024, 01, 03, 10, 0, 0),
            new DateTime(2024, 01, 03, 11, 0, 0),
            new DateTime(2024, 01, 03, 12, 0, 0),
            new DateTime(2024, 01, 15, 9, 0, 0),
            new DateTime(2024, 01, 15, 10, 0, 0),
            new DateTime(2024, 01, 15, 11, 0, 0),
            new DateTime(2024, 01, 15, 12, 0, 0)
        };

        // Act
        var result = _weeklyCalculatorService.CalculateWeeklyExecutions(config, 12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void CalculateWeeklyExecutions_ShouldReturnCorrectExecutions_ForEveryWeek()
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            WeekInterval = 1,
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Friday },
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 14)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent)
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01, 9, 0, 0),
            new DateTime(2024, 01, 01, 10, 0, 0),
            new DateTime(2024, 01, 01, 11, 0, 0),
            new DateTime(2024, 01, 01, 12, 0, 0),
            new DateTime(2024, 01, 05, 9, 0, 0),
            new DateTime(2024, 01, 05, 10, 0, 0),
            new DateTime(2024, 01, 05, 11, 0, 0),
            new DateTime(2024, 01, 05, 12, 0, 0),
            new DateTime(2024, 01, 08, 9, 0, 0),
            new DateTime(2024, 01, 08, 10, 0, 0),
            new DateTime(2024, 01, 08, 11, 0, 0),
            new DateTime(2024, 01, 08, 12, 0, 0),
        };

        // Act
        var result = _weeklyCalculatorService.CalculateWeeklyExecutions(config, 12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void CalculateWeeklyExecutions_ShouldReturnCorrectExecutions_ForEveryTwoWeeks()
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            WeekInterval = 2,
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Thursday },
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 02, 28)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent)
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 02, 9, 0, 0),
            new DateTime(2024, 01, 02, 10, 0, 0),
            new DateTime(2024, 01, 02, 11, 0, 0),
            new DateTime(2024, 01, 02, 12, 0, 0),
            new DateTime(2024, 01, 04, 9, 0, 0),
            new DateTime(2024, 01, 04, 10, 0, 0),
            new DateTime(2024, 01, 04, 11, 0, 0),
            new DateTime(2024, 01, 04, 12, 0, 0),
            new DateTime(2024, 01, 16, 9, 0, 0),
            new DateTime(2024, 01, 16, 10, 0, 0),
            new DateTime(2024, 01, 16, 11, 0, 0),
            new DateTime(2024, 01, 16, 12, 0, 0),
        };

        // Act
        var result = _weeklyCalculatorService.CalculateWeeklyExecutions(config, 12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void CalculateWeeklyExecutions_ShouldReturnCorrectExecutions_ForEveryThreeWeeks()
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            WeekInterval = 3,
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Wednesday },
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 03, 31)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent)
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 03, 9, 0, 0),
            new DateTime(2024, 01, 03, 10, 0, 0),
            new DateTime(2024, 01, 03, 11, 0, 0),
            new DateTime(2024, 01, 03, 12, 0, 0),
            new DateTime(2024, 01, 24, 9, 0, 0),
            new DateTime(2024, 01, 24, 10, 0, 0),
            new DateTime(2024, 01, 24, 11, 0, 0),
            new DateTime(2024, 01, 24, 12, 0, 0),
            new DateTime(2024, 02, 14, 9, 0, 0),
            new DateTime(2024, 02, 14, 10, 0, 0),
            new DateTime(2024, 02, 14, 11, 0, 0),
            new DateTime(2024, 02, 14, 12, 0, 0)
        };

        // Act
        var result = _weeklyCalculatorService.CalculateWeeklyExecutions(config, 12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void CalculateWeeklyExecutions_ShouldReturnEmpty_ForNoDaysSpecified()
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            WeekInterval = 1,
            DaysOfWeek = new List<DayOfWeek>(), // Sin días especificados
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 31)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        var result = _weeklyCalculatorService.CalculateWeeklyExecutions(config, 12).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData(1, new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Friday }, "2024-01-01 09:00:00,2024-01-01 10:00:00,2024-01-01 11:00:00,2024-01-01 12:00:00,2024-01-05 09:00:00,2024-01-05 10:00:00,2024-01-05 11:00:00,2024-01-05 12:00:00,2024-01-08 09:00:00,2024-01-08 10:00:00,2024-01-08 11:00:00,2024-01-08 12:00:00", "2024-01-01", "2024-01-14")]
    [InlineData(2, new DayOfWeek[] { DayOfWeek.Tuesday, DayOfWeek.Thursday }, "2024-01-02 09:00:00,2024-01-02 10:00:00,2024-01-02 11:00:00,2024-01-02 12:00:00,2024-01-04 09:00:00,2024-01-04 10:00:00,2024-01-04 11:00:00,2024-01-04 12:00:00,2024-01-16 09:00:00,2024-01-16 10:00:00,2024-01-16 11:00:00,2024-01-16 12:00:00", "2024-01-01", "2024-02-28")]
    [InlineData(3, new DayOfWeek[] { DayOfWeek.Wednesday }, "2024-01-03 09:00:00,2024-01-03 10:00:00,2024-01-03 11:00:00,2024-01-03 12:00:00,2024-01-24 09:00:00,2024-01-24 10:00:00,2024-01-24 11:00:00,2024-01-24 12:00:00,2024-02-14 09:00:00,2024-02-14 10:00:00,2024-02-14 11:00:00,2024-02-14 12:00:00", "2024-01-01", "2024-03-31")]
    public void CalculateWeeklyExecutions_ShouldReturnCorrectExecutions(int weekInterval, DayOfWeek[] daysOfWeek, string expectedDates, string startDate, string endDate)
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = DateTime.Parse(startDate),
            WeekInterval = weekInterval,
            DaysOfWeek = new List<DayOfWeek>(daysOfWeek),
            Limits = new LimitsTimeInterval(DateTime.Parse(startDate), DateTime.Parse(endDate)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent)
        };
        var expectedTimes = expectedDates.Split(',').Select(date => DateTime.Parse(date)).ToList();

        // Act
        var result = _weeklyCalculatorService.CalculateWeeklyExecutions(config, 12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Theory]
    [InlineData("2024-01-01", "2024-01-01", "Value cannot be null.")]
    public void CalculateWeeklyExecutions_ShouldThrowException_ForInvalidConfiguration(string startDate, string endDate, string expectedMessage)
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = DateTime.Parse(startDate),
            WeekInterval = 1,
            DaysOfWeek = null,
            Limits = new LimitsTimeInterval(DateTime.Parse(startDate), DateTime.Parse(endDate)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        var exception = Record.Exception(() => _weeklyCalculatorService.CalculateWeeklyExecutions(config, 12).ToList());

        // Assert
        exception.Should().BeOfType<ArgumentNullException>().Which.Message.Should().Contain(expectedMessage);
    }

    [Fact]
    public void CalculateWeeklyExecutions_ShouldRespectTimeLimits()
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            WeekInterval = 1,
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 10)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent)
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01, 9, 0, 0),
            new DateTime(2024, 01, 01, 10, 0, 0),
            new DateTime(2024, 01, 01, 11, 0, 0),
            new DateTime(2024, 01, 01, 12, 0, 0),
            new DateTime(2024, 01, 03, 9, 0, 0),
            new DateTime(2024, 01, 03, 10, 0, 0),
            new DateTime(2024, 01, 03, 11, 0, 0),
            new DateTime(2024, 01, 03, 12, 0, 0),
            new DateTime(2024, 01, 08, 9, 0, 0),
            new DateTime(2024, 01, 08, 10, 0, 0),
            new DateTime(2024, 01, 08, 11, 0, 0),
            new DateTime(2024, 01, 08, 12, 0, 0)
        };

        // Act
        var result = _weeklyCalculatorService.CalculateWeeklyExecutions(config, 12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }
}
