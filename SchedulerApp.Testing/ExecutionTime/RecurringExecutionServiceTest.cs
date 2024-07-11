using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Common.Validator;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Services.ExecutionCalculator;
using SchedulerApplication.Services.ExecutionTime;
using SchedulerApplication.Services.Interfaces;
using SchedulerApplication.ValueObjects;

public class RecurringExecutionServiceTests
{
    private readonly IConfigurationValidator _validatorService;
    private readonly IDailyExecutionCalculatorService _dailyExecutionCalculatorService;
    private readonly IWeeklyExecutionCalculatorService _weeklyExecutionCalculatorService;
    private readonly IRecurringExecutionService _recurringExecutionService;

    public RecurringExecutionServiceTests()
    {
        _validatorService = new ConfigurationValidator();
        _dailyExecutionCalculatorService = new DailyExecutionCalculatorService();
        _weeklyExecutionCalculatorService = new WeeklyExecutionCalculatorService(_dailyExecutionCalculatorService);
        _recurringExecutionService = new RecurringExecutionService(
            _validatorService,
            _dailyExecutionCalculatorService,
            _weeklyExecutionCalculatorService);
    }

    [Fact]
    public void CalculateNextExecutionTimes_DailyOccursOnce_ShouldReturnCorrectTimes()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            OccursOnce = true,
            OnceAt = new TimeSpan(9, 0, 0),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 12)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0)) 
        };

        // Act
        var result = _recurringExecutionService.CalculateNextExecutionTimes(configuration);

        // Assert
        result.Should().HaveCount(11);
        for (int i = 0; i < 11; i++)
        {
            result[i].Should().Be(new DateTime(2024, 01, 01).AddDays(i).Add(new TimeSpan(9, 0, 0)));
        }
    }

    [Fact]
    public void CalculateNextExecutionTimes_DailyRecurrent_ShouldReturnCorrectTimes()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            OccursOnce = false,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 02, 13, 0, 0))
        };

        // Act
        var result = _recurringExecutionService.CalculateNextExecutionTimes(configuration);

        // Assert
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01, 9, 0, 0),
            new DateTime(2024, 01, 01, 10, 0, 0),
            new DateTime(2024, 01, 01, 11, 0, 0),
            new DateTime(2024, 01, 01, 12, 0, 0),
            new DateTime(2024, 01, 02, 9, 0, 0),
            new DateTime(2024, 01, 02, 10, 0, 0),
            new DateTime(2024, 01, 02, 11, 0, 0),
            new DateTime(2024, 01, 02, 12, 0, 0)
        };

        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void CalculateNextExecutionTimes_Weekly_ShouldReturnCorrectTimes()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 31)),
            WeekInterval = 1,
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        var result = _recurringExecutionService.CalculateNextExecutionTimes(configuration);

        // Assert
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

        result.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void CalculateNextExecutionTimes_WeeklyInterval_ShouldReturnCorrectTimes()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 31)),
            WeekInterval = 2,
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday },
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        var result = _recurringExecutionService.CalculateNextExecutionTimes(configuration);

        // Assert
        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 01, 01, 9, 0, 0),
            new DateTime(2024, 01, 01, 10, 0, 0),
            new DateTime(2024, 01, 01, 11, 0, 0),
            new DateTime(2024, 01, 01, 12, 0, 0),
            new DateTime(2024, 01, 15, 9, 0, 0),
            new DateTime(2024, 01, 15, 10, 0, 0),
            new DateTime(2024, 01, 15, 11, 0, 0),
            new DateTime(2024, 01, 15, 12, 0, 0),
            new DateTime(2024, 01, 29, 9, 0, 0),
            new DateTime(2024, 01, 29, 10, 0, 0),
            new DateTime(2024, 01, 29, 11, 0, 0),
            new DateTime(2024, 01, 29, 12, 0, 0)
        };

        result.Should().BeEquivalentTo(expectedTimes);
    }
}