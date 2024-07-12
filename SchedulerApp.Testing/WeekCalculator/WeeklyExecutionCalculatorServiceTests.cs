using FluentAssertions;
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
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 31)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0)),
            HourlyInterval = 1
        };
        var expectedTimes = new List<DateTime>
        {
            new (2024, 01, 01, 9, 0, 0),
            new (2024, 01, 01, 10, 0, 0),
            new (2024, 01, 01, 11, 0, 0),
            new (2024, 01, 01, 12, 0, 0),
            new (2024, 01, 03, 9, 0, 0),
            new (2024, 01, 03, 10, 0, 0),
            new (2024, 01, 03, 11, 0, 0),
            new (2024, 01, 03, 12, 0, 0),
            new (2024, 01, 15, 9, 0, 0),
            new (2024, 01, 15, 10, 0, 0),
            new (2024, 01, 15, 11, 0, 0),
            new (2024, 01, 15, 12, 0, 0)
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
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Friday],
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 14)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0)),
            HourlyInterval = 1
        };
        var expectedTimes = new List<DateTime>
        {
            new (2024, 01, 01, 9, 0, 0),
            new (2024, 01, 01, 10, 0, 0),
            new (2024, 01, 01, 11, 0, 0),
            new (2024, 01, 01, 12, 0, 0),
            new (2024, 01, 05, 9, 0, 0),
            new (2024, 01, 05, 10, 0, 0),
            new (2024, 01, 05, 11, 0, 0),
            new (2024, 01, 05, 12, 0, 0),
            new (2024, 01, 08, 9, 0, 0),
            new (2024, 01, 08, 10, 0, 0),
            new (2024, 01, 08, 11, 0, 0),
            new (2024, 01, 08, 12, 0, 0),
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
            DaysOfWeek = [DayOfWeek.Tuesday, DayOfWeek.Thursday],
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 02, 28)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0)),
            HourlyInterval = 1
        };
        var expectedTimes = new List<DateTime>
        {
            new (2024, 01, 02, 9, 0, 0),
            new (2024, 01, 02, 10, 0, 0),
            new (2024, 01, 02, 11, 0, 0),
            new (2024, 01, 02, 12, 0, 0),
            new (2024, 01, 04, 9, 0, 0),
            new (2024, 01, 04, 10, 0, 0),
            new (2024, 01, 04, 11, 0, 0),
            new (2024, 01, 04, 12, 0, 0),
            new (2024, 01, 16, 9, 0, 0),
            new (2024, 01, 16, 10, 0, 0),
            new (2024, 01, 16, 11, 0, 0),
            new (2024, 01, 16, 12, 0, 0)
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
            DaysOfWeek = [DayOfWeek.Wednesday],
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 03, 31)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0)),
            HourlyInterval = 1
        };
        var expectedTimes = new List<DateTime>
        {
            new(2024, 01, 03, 9, 0, 0),
            new(2024, 01, 03, 10, 0, 0),
            new(2024, 01, 03, 11, 0, 0),
            new(2024, 01, 03, 12, 0, 0),
            new(2024, 01, 24, 9, 0, 0),
            new(2024, 01, 24, 10, 0, 0),
            new(2024, 01, 24, 11, 0, 0),
            new(2024, 01, 24, 12, 0, 0),
            new(2024, 02, 14, 9, 0, 0),
            new(2024, 02, 14, 10, 0, 0),
            new(2024, 02, 14, 11, 0, 0),
            new(2024, 02, 14, 12, 0, 0)
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
            DaysOfWeek = [], // Sin días especificados
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 31)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0)),
            HourlyInterval = 1
        };

        // Act
        var result = _weeklyCalculatorService.CalculateWeeklyExecutions(config, 12).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(WeeklyExecutionTestData))]
    public void CalculateWeeklyExecutions_ShouldReturnCorrectExecutions(
        int weekInterval,
        List<DayOfWeek> daysOfWeek,
        List<DateTime> expectedTimes,
        DateTime startDate,
        DateTime endDate)
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = startDate,
            WeekInterval = weekInterval,
            DaysOfWeek = daysOfWeek,
            Limits = new LimitsTimeInterval(startDate, endDate),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0)),
            HourlyInterval = 1
        };

        // Act
        var result = _weeklyCalculatorService.CalculateWeeklyExecutions(config, 12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Theory]
    [MemberData(nameof(InvalidConfigurationTestData))]
    public void CalculateWeeklyExecutions_ShouldThrowException_ForInvalidConfiguration(
        WeeklyFrequencyConfiguration config,
        string expectedMessage)
    {
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
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 10)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0)),
            HourlyInterval = 1
        };
        var expectedTimes = new List<DateTime>
        {
            new(2024, 01, 01, 9, 0, 0),
            new(2024, 01, 01, 10, 0, 0),
            new(2024, 01, 01, 11, 0, 0),
            new(2024, 01, 01, 12, 0, 0),
            new(2024, 01, 03, 9, 0, 0),
            new(2024, 01, 03, 10, 0, 0),
            new(2024, 01, 03, 11, 0, 0),
            new(2024, 01, 03, 12, 0, 0),
            new(2024, 01, 08, 9, 0, 0),
            new(2024, 01, 08, 10, 0, 0),
            new(2024, 01, 08, 11, 0, 0),
            new(2024, 01, 08, 12, 0, 0)
        };

        // Act
        var result = _weeklyCalculatorService.CalculateWeeklyExecutions(config, 12).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

    public static IEnumerable<object[]> WeeklyExecutionTestData =>
    new List<object[]>
    {
        new object[]
        {
            1,
            new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Friday },
            new List<DateTime>
            {
                new (2024, 01, 01, 9, 0, 0),
                new (2024, 01, 01, 10, 0, 0),
                new (2024, 01, 01, 11, 0, 0),
                new (2024, 01, 01, 12, 0, 0),
                new (2024, 01, 05, 9, 0, 0),
                new (2024, 01, 05, 10, 0, 0),
                new (2024, 01, 05, 11, 0, 0),
                new (2024, 01, 05, 12, 0, 0),
                new (2024, 01, 08, 9, 0, 0),
                new (2024, 01, 08, 10, 0, 0),
                new (2024, 01, 08, 11, 0, 0),
                new (2024, 01, 08, 12, 0, 0)
            },
            new DateTime(2024, 01, 01),
            new DateTime(2024, 01, 14)
        },
        new object[]
        {
            2,
            new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Thursday },
            new List<DateTime>
            {
                new (2024, 01, 02, 9, 0, 0),
                new (2024, 01, 02, 10, 0, 0),
                new (2024, 01, 02, 11, 0, 0),
                new (2024, 01, 02, 12, 0, 0),
                new (2024, 01, 04, 9, 0, 0),
                new (2024, 01, 04, 10, 0, 0),
                new (2024, 01, 04, 11, 0, 0),
                new (2024, 01, 04, 12, 0, 0),
                new (2024, 01, 16, 9, 0, 0),
                new (2024, 01, 16, 10, 0, 0),
                new (2024, 01, 16, 11, 0, 0),
                new (2024, 01, 16, 12, 0, 0)
            },
            new DateTime(2024, 01, 01),
            new DateTime(2024, 02, 28)
        },
        new object[]
        {
            3,
            new List<DayOfWeek> { DayOfWeek.Wednesday },
            new List<DateTime>
            {
                new (2024, 01, 03, 9, 0, 0),
                new (2024, 01, 03, 10, 0, 0),
                new (2024, 01, 03, 11, 0, 0),
                new (2024, 01, 03, 12, 0, 0),
                new (2024, 01, 24, 9, 0, 0),
                new (2024, 01, 24, 10, 0, 0),
                new (2024, 01, 24, 11, 0, 0),
                new (2024, 01, 24, 12, 0, 0),
                new (2024, 02, 14, 9, 0, 0),
                new (2024, 02, 14, 10, 0, 0),
                new (2024, 02, 14, 11, 0, 0),
                new (2024, 02, 14, 12, 0, 0)
            },
            new DateTime(2024, 01, 01),
            new DateTime(2024, 03, 31)
        }
    };

    public static IEnumerable<object[]> InvalidConfigurationTestData =>
    new List<object[]>
    {
        new object[]
        {
            new WeeklyFrequencyConfiguration
            {
                IsEnabled = true,
                CurrentDate = new DateTime(2024, 01, 01),
                WeekInterval = 1,
                DaysOfWeek = null,
                Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 31)),
                HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0)),
                HourlyInterval = 1
            },
            "Value cannot be null."
        }
    };
}
