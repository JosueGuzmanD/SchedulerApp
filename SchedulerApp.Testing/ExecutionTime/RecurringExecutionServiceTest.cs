using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Common.Validator;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.ExecutionTime;
using SchedulerApplication.Services.HourCalculator;
using SchedulerApplication.Services.Interfaces;
using SchedulerApplication.Services.WeekCalculator;
using SchedulerApplication.ValueObjects;

public class RecurringExecutionServiceTests
{
    private readonly IConfigurationValidator _validator;
    private readonly IHourCalculatorService _hourCalculator;
    private readonly IWeekCalculatorService _weekCalculator;
    private readonly RecurringExecutionService _service;

    public RecurringExecutionServiceTests()
    {
        _validator = new ConfigurationValidator();
        _hourCalculator = new HourCalculatorService();
        _weekCalculator = new WeekCalculatorService();
        _service = new RecurringExecutionService(_validator, _hourCalculator, _weekCalculator);
    }

    [Fact]
    public void CalculateNextExecutionTime_ShouldReturnThreeExecutionTimes_WhenValidConfiguration()
    {
        // Arrange
        var configuration = new RecurringSchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 1, 1),
            IsEnabled = true,
            TimeInterval = new TimeInterval(new DateTime(2024, 1, 1), new DateTime(2024, 12, 31)),
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
            WeekInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        var result = _service.CalculateNextExecutionTime(configuration);

        // Assert
        result.Should().HaveCount(3); // Limiting to 3 executions
    }

    [Fact]
    public void CalculateNextExecutionTime_ShouldNotExceedMaxExecutions()
    {
        // Arrange
        var configuration = new RecurringSchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 1, 1),
            IsEnabled = true,
            TimeInterval = new TimeInterval(new DateTime(2024, 1, 1), new DateTime(2024, 12, 31)),
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
            WeekInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        var result = _service.CalculateNextExecutionTime(configuration);

        // Assert
        result.Should().HaveCount(3); // Should not exceed 3 executions
    }

    [Fact]
    public void CalculateNextExecutionTime_ShouldReturnEmptyList_WhenNoValidExecutionTimes()
    {
        // Arrange
        var configuration = new RecurringSchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 1, 1),
            IsEnabled = true,
            TimeInterval = new TimeInterval(new DateTime(2025, 1, 1), new DateTime(2025, 12, 31)),
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
            WeekInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        var result = _service.CalculateNextExecutionTime(configuration);

        // Assert
        result.Should().BeEmpty(); // No valid execution times within the interval
    }

    [Fact]
    public void CalculateNextExecutionTime_ShouldHandleSingleExecutionPerDay()
    {
        // Arrange
        var configuration = new RecurringSchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 1, 1),
            IsEnabled = true,
            TimeInterval = new TimeInterval(new DateTime(2024, 1, 1), new DateTime(2024, 12, 31)),
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
            WeekInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(9, 0, 0), 1, DailyHourFrequency.Once)
        };

        // Act
        var result = _service.CalculateNextExecutionTime(configuration);

        // Assert
        result.Should().HaveCount(3); // Only three executions limited by maxExecutions
        result.All(dt => dt.TimeOfDay == new TimeSpan(9, 0, 0)).Should().BeTrue();
    }

    [Theory]
    [InlineData("2024-01-01", new DayOfWeek[] { DayOfWeek.Monday }, 1, new string[] { "2024-01-01 09:00:00", "2024-01-08 09:00:00", "2024-01-15 09:00:00" })]
    [InlineData("2024-01-01", new DayOfWeek[] { DayOfWeek.Wednesday }, 2, new string[] { "2024-01-03 09:00:00", "2024-01-17 09:00:00", "2024-01-31 09:00:00" })]
    public void CalculateNextExecutionTime_ShouldReturnCorrectExecutionTimes(
        string initialDate, DayOfWeek[] daysOfWeek, int weekInterval, string[] expectedDates)
    {
        // Arrange
        var configuration = new RecurringSchedulerConfiguration
        {
            CurrentDate = DateTime.Parse(initialDate),
            IsEnabled = true,
            TimeInterval = new TimeInterval(new DateTime(2024, 1, 1), new DateTime(2024, 12, 31)),
            DaysOfWeek = daysOfWeek.ToList(),
            WeekInterval = weekInterval,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        var expectedExecutionDates = expectedDates.Select(DateTime.Parse).ToList();

        // Act
        var result = _service.CalculateNextExecutionTime(configuration);

        // Assert
        result.Should().BeEquivalentTo(expectedExecutionDates);
    }
}

