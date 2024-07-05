using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Common.Validator;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Services.Description;
using SchedulerApplication.Services.ExecutionTime;
using SchedulerApplication.Services.HourCalculator;
using SchedulerApplication.Services.Interfaces;
using SchedulerApplication.Services.ScheduleTypes;
using SchedulerApplication.Services.WeekCalculator;
using SchedulerApplication.ValueObjects;

namespace SchedulerApp.Testing.ScheduleTypes;

public class ScheduleTypeRecurringTests
{
    private readonly IDescriptionService _descriptionService;
    private readonly IRecurringExecutionService _recurringExecutionService;
    private readonly ScheduleTypeRecurring _scheduleTypeRecurring;

    public ScheduleTypeRecurringTests()
    {
        _descriptionService = new DescriptionService();
        _recurringExecutionService = new RecurringExecutionService(new ConfigurationValidator(), new HourCalculatorService(), new WeekCalculatorService());
        _scheduleTypeRecurring = new ScheduleTypeRecurring(_descriptionService, _recurringExecutionService);
    }

    [Fact]
    public void CreateScheduleOutput_ShouldReturnCorrectOutputs_WhenConfigurationIsValid()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            TimeInterval = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
            WeekInterval = 1,
            DaysInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        var result = _scheduleTypeRecurring.GetNextExecutionTimes(configuration);

        // Assert
        result.Should().HaveCount(12);
        result[0].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 09, 00, 00));
        result[1].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 10, 00, 00));
        result[2].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 11, 00, 00));
        result[3].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 12, 00, 00));
        result[4].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 13, 00, 00));
        result[5].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 14, 00, 00));
        result[6].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 15, 00, 00));
        result[7].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 16, 00, 00));
        result[8].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 17, 00, 00));
        result[9].ExecutionTime.Should().Be(new DateTime(2024, 01, 08, 09, 00, 00));
        result[10].ExecutionTime.Should().Be(new DateTime(2024, 01, 08, 10, 00, 00));
        result[11].ExecutionTime.Should().Be(new DateTime(2024, 01, 08, 11, 00, 00));
    }

    [Fact]
    public void CreateScheduleOutput_ShouldThrowException_WhenConfigurationIsInvalid()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = false, 
            TimeInterval = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
            WeekInterval = 1,
            DaysInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        Action act = () => _scheduleTypeRecurring.GetNextExecutionTimes(configuration);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Configuration must be enabled.");
    }

    [Fact]
    public void CreateScheduleOutput_ShouldHandleDailyFrequencyConfiguration()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            TimeInterval = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            DaysInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        var result = _scheduleTypeRecurring.GetNextExecutionTimes(configuration);

        // Assert
        result.Should().HaveCount(12);
        result[0].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 09, 00, 00));
        result[1].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 10, 00, 00));
        result[2].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 11, 00, 00));
        result[3].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 12, 00, 00));
        result[4].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 13, 00, 00));
        result[5].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 14, 00, 00));
        result[6].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 15, 00, 00));
        result[7].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 16, 00, 00));
        result[8].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 17, 00, 00));
        result[9].ExecutionTime.Should().Be(new DateTime(2024, 01, 02, 09, 00, 00));
        result[10].ExecutionTime.Should().Be(new DateTime(2024, 01, 02, 10, 00, 00));
        result[11].ExecutionTime.Should().Be(new DateTime(2024, 01, 02, 11, 00, 00));
    }

    [Fact]
    public void CreateScheduleOutput_ShouldHandleMidnightTransition_ForDailyFrequency()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            TimeInterval = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            DaysInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(23, 0, 0), new TimeSpan(2, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        var result = _scheduleTypeRecurring.GetNextExecutionTimes(configuration);

        // Assert
        result.Should().HaveCount(12);
        result.Should().Contain(x => x.ExecutionTime == new DateTime(2024, 01, 01, 23, 00, 00));
        result.Should().Contain(x => x.ExecutionTime == new DateTime(2024, 01, 02, 00, 00, 00));
        result.Should().Contain(x => x.ExecutionTime == new DateTime(2024, 01, 02, 01, 00, 00));
        result.Should().Contain(x => x.ExecutionTime == new DateTime(2024, 01, 02, 02, 00, 00));
    }

    [Theory]
    [InlineData("2024-01-01", new[] { DayOfWeek.Monday, DayOfWeek.Wednesday }, 1, 12)]
    [InlineData("2024-01-01", new[] { DayOfWeek.Friday }, 2, 12)]
    [InlineData("2024-01-01", new[] { DayOfWeek.Tuesday, DayOfWeek.Thursday }, 1, 12)]
    public void CreateScheduleOutput_ShouldReturnCorrectExecutionTimes(string initialDateString, DayOfWeek[] daysOfWeek, int weekInterval, int expectedCount)
    {
        // Arrange
        var initialDate = DateTime.Parse(initialDateString);
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = initialDate,
            IsEnabled = true,
            TimeInterval = new LimitsTimeInterval(initialDate, initialDate.AddMonths(1)),
            DaysOfWeek = new List<DayOfWeek>(daysOfWeek),
            WeekInterval = weekInterval,
            DaysInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        var result = _scheduleTypeRecurring.GetNextExecutionTimes(configuration);

        // Assert
        result.Should().HaveCount(expectedCount);
    }

    [Fact]
    public void CreateScheduleOutput_ShouldReturnCorrectOutputs_ForWeeklyFrequencyConfiguration()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            TimeInterval = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
            WeekInterval = 1,
            DaysInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        var result = _scheduleTypeRecurring.GetNextExecutionTimes(configuration);

        // Assert
        result.Should().HaveCount(12);
        result[0].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 09, 00, 00));
        result[1].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 10, 00, 00));
        result[2].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 11, 00, 00));
        result[3].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 12, 00, 00));
        result[4].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 13, 00, 00));
        result[5].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 14, 00, 00));
        result[6].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 15, 00, 00));
        result[7].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 16, 00, 00));
        result[8].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 17, 00, 00));
        result[9].ExecutionTime.Should().Be(new DateTime(2024, 01, 08, 09, 00, 00));
        result[10].ExecutionTime.Should().Be(new DateTime(2024, 01, 08, 10, 00, 00));
        result[11].ExecutionTime.Should().Be(new DateTime(2024, 01, 08, 11, 00, 00));
    }
}
