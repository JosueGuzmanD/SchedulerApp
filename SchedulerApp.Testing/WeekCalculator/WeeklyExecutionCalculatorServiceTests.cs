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
    private readonly IWeeklyExecutionCalculatorService _weeklyCalculatorService;

    public WeeklyExecutionCalculatorServiceTests()
    {
        _weeklyCalculatorService = new WeeklyExecutionCalculatorService();
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
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 31))
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01),
            new DateTime(2024, 01, 03),
            new DateTime(2024, 01, 15),
            new DateTime(2024, 01, 17),
            new DateTime(2024, 01, 29),
            new DateTime(2024, 01, 31)
        };

        // Act
        var result = _weeklyCalculatorService.CalculateWeeklyExecutions(config).ToList();

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
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 14))
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01),
            new DateTime(2024, 01, 05),
            new DateTime(2024, 01, 08),
            new DateTime(2024, 01, 12)
        };

        // Act
        var result = _weeklyCalculatorService.CalculateWeeklyExecutions(config).ToList();

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
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 02, 28))
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 02),
            new DateTime(2024, 01, 04),
            new DateTime(2024, 01, 16),
            new DateTime(2024, 01, 18),
            new DateTime(2024, 01, 30),
            new DateTime(2024, 02, 01),
            new DateTime(2024, 02, 13),
            new DateTime(2024, 02, 15),
            new DateTime(2024, 02, 27),
        };

        // Act
        var result = _weeklyCalculatorService.CalculateWeeklyExecutions(config).ToList();

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
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 03, 31))
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 03),
            new DateTime(2024, 01, 24),
            new DateTime(2024, 02, 14),
            new DateTime(2024, 03, 06),
            new DateTime(2024, 03, 27)
        };

        // Act
        var result = _weeklyCalculatorService.CalculateWeeklyExecutions(config).ToList();

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
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 31))
        };

        // Act
        var result = _weeklyCalculatorService.CalculateWeeklyExecutions(config).ToList();

        // Assert
        result.Should().BeEmpty();
    }
    [Theory]
    [InlineData(1, new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Friday }, "2024-01-01,2024-01-05,2024-01-08,2024-01-12", "2024-01-01", "2024-01-14")]
    [InlineData(2, new DayOfWeek[] { DayOfWeek.Tuesday, DayOfWeek.Thursday }, "2024-01-02,2024-01-04,2024-01-16,2024-01-18,2024-01-30,2024-02-01,2024-02-13,2024-02-15,2024-02-27", "2024-01-01", "2024-02-28")]
    [InlineData(3, new DayOfWeek[] { DayOfWeek.Wednesday }, "2024-01-03,2024-01-24,2024-02-14,2024-03-06,2024-03-27", "2024-01-01", "2024-03-31")]
    public void CalculateWeeklyExecutions_ShouldReturnCorrectExecutions(int weekInterval, DayOfWeek[] daysOfWeek, string expectedDates, string startDate, string endDate)
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = DateTime.Parse(startDate),
            WeekInterval = weekInterval,
            DaysOfWeek = new List<DayOfWeek>(daysOfWeek),
            Limits = new LimitsTimeInterval(DateTime.Parse(startDate), DateTime.Parse(endDate))
        };
        var expectedTimes = expectedDates.Split(',').Select(date => DateTime.Parse(date)).ToList();

        // Act
        var result = _weeklyCalculatorService.CalculateWeeklyExecutions(config).ToList();

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
            Limits = new LimitsTimeInterval(DateTime.Parse(startDate), DateTime.Parse(endDate))
        };

        // Act
        var exception = Record.Exception(() => _weeklyCalculatorService.CalculateWeeklyExecutions(config).ToList());

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
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 10))
        };
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01),
            new DateTime(2024, 01, 03),
            new DateTime(2024, 01, 08),
            new DateTime(2024, 01, 10)
        };

        // Act
        var result = _weeklyCalculatorService.CalculateWeeklyExecutions(config).ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedTimes);
    }

}