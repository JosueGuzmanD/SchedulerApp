using FluentAssertions;
using SchedulerApplication.Common.Validator;
using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.ValueObjects;
using SchedulerApplication.Services.DateCalculatorServices;
using SchedulerApplication.Services.Description;
using SchedulerApplication.Services.ExecutionTime;
using SchedulerApplication.Services.ScheduleTypes;

namespace SchedulerApp.Testing.ScheduleTypes;

public class ScheduleTypeRecurringTests
{
    private readonly IRecurringExecutionService _recurringExecutionService;
    private readonly IDescriptionService _descriptionService;
    private readonly ScheduleTypeRecurring _scheduleTypeRecurring;

    public ScheduleTypeRecurringTests()
    {
        var validatorService = new ConfigurationValidator();
        var dailyExecutionCalculatorService = new DailyExecutionCalculatorService();
        var weeklyExecutionCalculatorService = new WeeklyExecutionCalculatorService(dailyExecutionCalculatorService);

        _recurringExecutionService = new RecurringExecutionService(
            validatorService,
            dailyExecutionCalculatorService,
            weeklyExecutionCalculatorService);

        _descriptionService = new DescriptionService();
        _scheduleTypeRecurring = new ScheduleTypeRecurring(_descriptionService, _recurringExecutionService);
    }

    [Fact]
    public void GetNextExecutionTimes_DailyOccursOnce_ShouldReturnCorrectOutput()
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
        var result = _scheduleTypeRecurring.GetNextExecutionTimes(configuration);

        // Assert
        var expectedTimes = Enumerable.Range(0, 11)
            .Select(i => new DateTime(2024, 01, 01).AddDays(i).Add(new TimeSpan(9, 0, 0)))
            .ToList();

        result.Should().HaveCount(11);
        for (var i = 0; i < 11; i++)
        {
            result[i].ExecutionTime.Should().Be(expectedTimes[i]);
            result[i].Description.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public void GetNextExecutionTimes_DailyRecurrent_ShouldReturnCorrectOutput()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            OccursOnce = false,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0)),
            HourlyInterval = 1,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 02, 13, 0, 0))
        };

        // Act
        var result = _scheduleTypeRecurring.GetNextExecutionTimes(configuration);

        // Assert
        var expectedTimes = new List<DateTime>
        {
            new (2024, 01, 01, 9, 0, 0),
            new (2024, 01, 01, 10, 0, 0),
            new (2024, 01, 01, 11, 0, 0),
            new (2024, 01, 01, 12, 0, 0),
            new (2024, 01, 02, 9, 0, 0),
            new (2024, 01, 02, 10, 0, 0),
            new (2024, 01, 02, 11, 0, 0),
            new (2024, 01, 02, 12, 0, 0)
        };

        result.Should().HaveCount(8);
        for (var i = 0; i < 8; i++)
        {
            result[i].ExecutionTime.Should().Be(expectedTimes[i]);
            result[i].Description.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public void GetNextExecutionTimes_Weekly_ShouldReturnCorrectOutput()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 31)),
            WeekInterval = 1,
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0)),
            HourlyInterval = 1
        };

        // Act
        var result = _scheduleTypeRecurring.GetNextExecutionTimes(configuration);

        // Assert
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
            new (2024, 01, 08, 9, 0, 0),
            new (2024, 01, 08, 10, 0, 0),
            new (2024, 01, 08, 11, 0, 0),
            new (2024, 01, 08, 12, 0, 0)
        };

        result.Should().HaveCount(12);
        for (var i = 0; i < 12; i++)
        {
            result[i].ExecutionTime.Should().Be(expectedTimes[i]);
            result[i].Description.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public void GetNextExecutionTimes_WeeklyInterval_ShouldReturnCorrectOutput()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 31)),
            WeekInterval = 2,
            DaysOfWeek = [DayOfWeek.Monday],
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0)),
            HourlyInterval = 1
        };

        // Act
        var result = _scheduleTypeRecurring.GetNextExecutionTimes(configuration);

        // Assert
        var expectedTimes = new List<DateTime>
        {
            new (2024, 01, 01, 9, 0, 0),
            new (2024, 01, 01, 10, 0, 0),
            new (2024, 01, 01, 11, 0, 0),
            new (2024, 01, 01, 12, 0, 0),
            new (2024, 01, 15, 9, 0, 0),
            new (2024, 01, 15, 10, 0, 0),
            new (2024, 01, 15, 11, 0, 0),
            new (2024, 01, 15, 12, 0, 0),
            new (2024, 01, 29, 9, 0, 0),
            new (2024, 01, 29, 10, 0, 0),
            new (2024, 01, 29, 11, 0, 0),
            new (2024, 01, 29, 12, 0, 0)
        };

        result.Should().HaveCount(12);
        for (var i = 0; i < 12; i++)
        {
            result[i].ExecutionTime.Should().Be(expectedTimes[i]);
            result[i].Description.Should().NotBeNullOrEmpty();
        }
    }
}
