using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Interfaces;
using SchedulerApplication.Models;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Models.ValueObjects;
using SchedulerApplication.Services.DateCalculator;
using SchedulerApplication.Services.DayOptionStrategies;
using SchedulerApplication.Services.ExecutionTime;
using SchedulerApplication.Services.HourCalculator;
using SchedulerApplication.Services.ScheduleTypes;
using System.Globalization;

namespace SchedulerApp.Testing.ScheduleTypesTest;

public class ScheduleTypeTest
{
    private readonly ScheduleTypeFactory _factory;
    private readonly IDescriptionService _descriptionService;
    private readonly IExecutionTimeGenerator _timeGenerator;
    private CustomStringLocalizer localizer = new CustomStringLocalizer();

    public ScheduleTypeTest()
    {
        _descriptionService = new DescriptionService(localizer);
        _timeGenerator = new ExecutionTimeGenerator(
            new OnceDateCalculator(),
            new DailyDateCalculator(),
            new WeeklyDateCalculator(),
            new HourCalculatorService(),
            new MonthlyDateCalculator(),
            localizer);
        _factory = new ScheduleTypeFactory(_descriptionService, _timeGenerator);
    }

    [Fact]
    public void CreateSchedule_ShouldReturnCorrectExecution_WhenOnceSchedulerConfigurationIsCorrect()
    {
        var configuration = new OnceSchedulerConfiguration
        {
            ConfigurationDateTime = new DateTime(2024, 07, 15),
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 02, 09, 00, 00),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31))
        };

        var scheduleType = _factory.CreateScheduleType(configuration, 12);
        var executionTimes = scheduleType.GetNextExecutionTimes(configuration);

        scheduleType.Should().BeOfType<ScheduleTypeOnce>();
        executionTimes.Should().HaveCount(1);
        executionTimes[0].ExecutionTime.Should().Be(new DateTime(2024, 07, 15));
        executionTimes[0].Description.Should().Be("Occurs once. Schedule will be used on 15/07/2024 at 00:00 starting on 02/01/2024.");
    }

    [Fact]
    public void CreateSchedule_ShouldReturnCorrectExecution_WhenDailyFrequencyConfigurationIsCorrect()
    {
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 07, 15, 09, 02, 21),
            Interval = 2,
            IntervalType = IntervalType.Hourly,
            HourTimeRange = new HourTimeRange(new TimeSpan(21, 00, 00), new TimeSpan(23, 00, 00)),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 07, 15), new DateTime(2024, 07, 17))
        };

        var scheduleType = _factory.CreateScheduleType(configuration, 12);
        var executionTimes = scheduleType.GetNextExecutionTimes(configuration);

        scheduleType.Should().BeOfType<ScheduleTypeRecurring>();
        executionTimes.Should().HaveCount(4);

        executionTimes[0].ExecutionTime.Should().Be(new DateTime(2024, 07, 15, 21, 00, 00));
        executionTimes[0].Description.Should().Be($@"Occurs every day from {configuration.HourTimeRange.StartHour:hh\:mm} to {configuration.HourTimeRange.EndHour:hh\:mm}. Schedule will be used on {new DateTime(2024, 07, 15, 21, 00, 00):dd/MM/yyyy} at {new DateTime(2024, 07, 15, 21, 00, 00):HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");

        executionTimes[1].ExecutionTime.Should().Be(new DateTime(2024, 07, 15, 23, 00, 00));
        executionTimes[1].Description.Should().Be($@"Occurs every day from {configuration.HourTimeRange.StartHour:hh\:mm} to {configuration.HourTimeRange.EndHour:hh\:mm}. Schedule will be used on {new DateTime(2024, 07, 15, 23, 00, 00):dd/MM/yyyy} at {new DateTime(2024, 07, 15, 23, 00, 00):HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");

        executionTimes[2].ExecutionTime.Should().Be(new DateTime(2024, 07, 16, 21, 00, 00));
        executionTimes[2].Description.Should().Be($@"Occurs every day from {configuration.HourTimeRange.StartHour:hh\:mm} to {configuration.HourTimeRange.EndHour:hh\:mm}. Schedule will be used on {new DateTime(2024, 07, 16, 21, 00, 00):dd/MM/yyyy} at {new DateTime(2024, 07, 16, 21, 00, 00):HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");

        executionTimes[3].ExecutionTime.Should().Be(new DateTime(2024, 07, 16, 23, 00, 00));
        executionTimes[3].Description.Should().Be($@"Occurs every day from {configuration.HourTimeRange.StartHour:hh\:mm} to {configuration.HourTimeRange.EndHour:hh\:mm}. Schedule will be used on {new DateTime(2024, 07, 16, 23, 00, 00):dd/MM/yyyy} at {new DateTime(2024, 07, 16, 23, 00, 00):HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
    }

    [Fact]
    public void CreateSchedule_ShouldReturnException_WhenConfigurationIsInvalid()
    {
        var configuration = new UnsupportedConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01))
        };

        Action act = () => _factory.CreateScheduleType(configuration, 12);

        act.Should().Throw<ArgumentException>("Unsupported configuration type.");
    }
    [Fact]
    public void CalculateHours_ShouldReturnCorrectHours_WithHourlyInterval()
    {
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 02)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Interval = 2,
            IntervalType = IntervalType.Hourly
        };

        var scheduleType = _factory.CreateScheduleType(configuration, 12).GetNextExecutionTimes(configuration);

        scheduleType.Should().HaveCount(5);
        scheduleType[0].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 9, 0, 0));
        scheduleType[1].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 11, 0, 0));
        scheduleType[2].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 13, 0, 0));
        scheduleType[3].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 15, 0, 0));
        scheduleType[4].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 17, 0, 0));
    }

    [Fact]
    public void CalculateHours_ShouldReturnCorrectMinutes_WithMinutelyInterval()
    {
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 02)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(9, 10, 0)),
            Interval = 5,
            IntervalType = IntervalType.Minutely
        };

        var scheduleType = _factory.CreateScheduleType(configuration, 12).GetNextExecutionTimes(configuration);

        scheduleType.Should().HaveCount(3);
        scheduleType[0].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 9, 0, 0));
        scheduleType[1].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 9, 5, 0));
        scheduleType[2].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 9, 10, 0));
    }

    [Fact]
    public void CalculateHours_ShouldReturnCorrectSeconds_WithSecondlyInterval()
    {
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 02)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(9, 0, 10)),
            Interval = 5,
            IntervalType = IntervalType.Secondly
        };

        var scheduleType = _factory.CreateScheduleType(configuration, 12).GetNextExecutionTimes(configuration);

        scheduleType.Should().HaveCount(3);
        scheduleType[0].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 9, 0, 0));
        scheduleType[1].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 9, 0, 5));
        scheduleType[2].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 9, 0, 10));
    }

    [Fact]
    public void CreateSchedule_ShouldReturnCorrectDescription_WhenOnceSchedulerConfigurationIsValid()
    {
        var configuration = new OnceSchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            ConfigurationDateTime = new DateTime(2024, 02, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 02, 01))
        };

        var scheduleType = _factory.CreateScheduleType(configuration, 12).GetNextExecutionTimes(configuration);


        scheduleType[0].Description.Should().Be($"Occurs once. Schedule will be used on {configuration.ConfigurationDateTime:dd/MM/yyyy} at {configuration.ConfigurationDateTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");

    }

    [Fact]
    public void CreateSchedule_ShouldReturnCorrectDescription_WhenDailyFrequencyConfigurationHourTimeRangeIsTheSame()
    {
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 03, 20, 00, 00)),
            HourTimeRange = new HourTimeRange(new TimeSpan(20, 00, 00), new TimeSpan(20, 00, 00)),
            Interval = 1,
            IntervalType = IntervalType.Hourly
        };

        var scheduleType = _factory.CreateScheduleType(configuration, 12).GetNextExecutionTimes(configuration);

        scheduleType.Should().HaveCount(3);
        scheduleType[0].ExecutionTime.Should().Be(new DateTime(2024, 03, 01, 20, 00, 00));
        scheduleType[0].Description.Should().Be($"Occurs every day from {configuration.HourTimeRange.StartHour:hh\\:mm} to {configuration.HourTimeRange.EndHour:hh\\:mm}. Schedule will be used on {new DateTime(2024, 03, 01, 20, 00, 00):dd/MM/yyyy} at {new DateTime(2024, 03, 01, 20, 00, 00):HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");

        scheduleType[1].ExecutionTime.Should().Be(new DateTime(2024, 03, 02, 20, 00, 00));
        scheduleType[1].Description.Should().Be($"Occurs every day from {configuration.HourTimeRange.StartHour:hh\\:mm} to {configuration.HourTimeRange.EndHour:hh\\:mm}. Schedule will be used on {new DateTime(2024, 03, 02, 20, 00, 00):dd/MM/yyyy} at {new DateTime(2024, 03, 02, 20, 00, 00):HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");

        scheduleType[2].ExecutionTime.Should().Be(new DateTime(2024, 03, 03, 20, 00, 00));
        scheduleType[2].Description.Should().Be($"Occurs every day from {configuration.HourTimeRange.StartHour:hh\\:mm} to {configuration.HourTimeRange.EndHour:hh\\:mm}. Schedule will be used on {new DateTime(2024, 03, 03, 20, 00, 00):dd/MM/yyyy} at {new DateTime(2024, 03, 03, 20, 00, 00):HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
    }
    [Fact]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForWeeklyFrequencyConfiguration()
    {
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
            WeekInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0))
        };
        var executionTime = new DateTime(2024, 03, 04, 9, 0, 0);

        var description = _descriptionService.GenerateDescription(configuration, executionTime);

        description.Should().Be("Occurs every 1 week(s) on Monday, Wednesday. Schedule will be used on 04/03/2024 at 09:00 starting on 01/03/2024.");
    }

    [Fact]
    public void GenerateExecutions_ShouldReturnCorrectExecutionTimes_ForOnceSchedulerConfiguration()
    {
        var configuration = new OnceSchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            ConfigurationDateTime = new DateTime(2024, 03, 05, 9, 0, 0),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 06))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        executionTimes.Should().HaveCount(1);
        executionTimes[0].Should().Be(new DateTime(2024, 03, 05, 9, 0, 0));
    }


    [Fact]
    public void GenerateExecutions_ShouldReturnCorrectExecutionTimes_ForDailyFrequencyConfiguration()
    {
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Interval = 2,
            IntervalType = 0,
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 03))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        var expectedTimes = new List<DateTime>
        {
            new (2024, 03, 01, 9, 0, 0),
            new (2024, 03, 01, 11, 0, 0),
            new (2024, 03, 01, 13, 0, 0),
            new (2024, 03, 01, 15, 0, 0),
            new (2024, 03, 01, 17, 0, 0),
            new (2024, 03, 02, 9, 0, 0),
            new (2024, 03, 02, 11, 0, 0),
            new (2024, 03, 02, 13, 0, 0),
            new (2024, 03, 02, 15, 0, 0),
            new (2024, 03, 02, 17, 0, 0),
        };

        executionTimes.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void GenerateExecutions_ShouldReturnCorrectExecutionTimes_ForWeeklyFrequencyConfiguration()
    {
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
            WeekInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Interval = 2,
            IntervalType = 0,
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 15))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        var expectedTimes = new List<DateTime>
        {
            new (2024, 03, 04, 9, 0, 0),
            new (2024, 03, 04, 11, 0, 0),
            new (2024, 03, 04, 13, 0, 0),
            new (2024, 03, 04, 15, 0, 0),
            new (2024, 03, 04, 17, 0, 0),
            new (2024, 03, 06, 9, 0, 0),
            new (2024, 03, 06, 11, 0, 0),
            new (2024, 03, 06, 13, 0, 0),
            new (2024, 03, 06, 15, 0, 0),
            new (2024, 03, 06, 17, 0, 0),
            new (2024, 03, 11, 9, 0, 0),
            new (2024, 03, 11, 11, 0, 0)
        };

        executionTimes.Should().BeEquivalentTo(expectedTimes);
    }
    [Fact]
    public void GenerateExecutions_ShouldReturnCorrectExecutionTimes_ForWeeklyEqualFrequencyConfiguration()
    {
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
            WeekInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Interval = 2,
            IntervalType = IntervalType.Hourly,
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 11, 9, 0, 0))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        var expectedTimes = new List<DateTime>
        {
            new (2024, 03, 04, 9, 0, 0),
            new (2024, 03, 04, 11, 0, 0),
            new (2024, 03, 04, 13, 0, 0),
            new (2024, 03, 04, 15, 0, 0),
            new (2024, 03, 04, 17, 0, 0),
            new (2024, 03, 06, 9, 0, 0),
            new (2024, 03, 06, 11, 0, 0),
            new (2024, 03, 06, 13, 0, 0),
            new (2024, 03, 06, 15, 0, 0),
            new (2024, 03, 06, 17, 0, 0),
            new (2024, 03, 11, 9, 0, 0)
        };

        executionTimes.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void OnceSchedulerConfiguration_ShouldReturnEmptyList_WhenDisabled()
    {
        var configuration = new OnceSchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = false,
            ConfigurationDateTime = new DateTime(2024, 03, 05, 9, 0, 0)
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        executionTimes.Should().BeEmpty();
    }
    [Fact]
    public void DailyFrequencyConfiguration_ShouldReturnCorrectExecutionTimes_WithDailyInterval()
    {
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            Interval = 2,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 03))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        var expectedTimes = new List<DateTime>
        {
            new (2024, 03, 01, 9, 0, 0),
            new (2024, 03, 01, 11, 0, 0),
            new (2024, 03, 01, 13, 0, 0),
            new (2024, 03, 01, 15, 0, 0),
            new (2024, 03, 01, 17, 0, 0),
            new (2024, 03, 02, 9, 0, 0),
            new (2024, 03, 02, 11, 0, 0),
            new (2024, 03, 02, 13, 0, 0),
            new (2024, 03, 02, 15, 0, 0),
            new (2024, 03, 02, 17, 0, 0),
        };

        executionTimes.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void WeeklyFrequencyConfiguration_ShouldReturnCorrectExecutionTimes_WithSpecificDaysOfWeek()
    {
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Friday],
            WeekInterval = 1,
            Interval = 2,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 15))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        var expectedTimes = new List<DateTime>
        {
            new(2024, 03, 01, 9, 0, 0),
            new(2024, 03, 01, 11, 0, 0),
            new(2024, 03, 01, 13, 0, 0),
            new(2024, 03, 01, 15, 0, 0),
            new(2024, 03, 01, 17, 0, 0),
            new(2024, 03, 04, 9, 0, 0),
            new(2024, 03, 04, 11, 0, 0),
            new(2024, 03, 04, 13, 0, 0),
            new(2024, 03, 04, 15, 0, 0),
            new(2024, 03, 04, 17, 0, 0),
            new(2024, 03, 08, 9, 0, 0),
            new(2024, 03, 08, 11, 0, 0)
        };

        executionTimes.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void WeeklyFrequencyConfiguration_ShouldReturnCorrectExecutionTimes_WithWeeklyInterval()
    {
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
            WeekInterval = 1,
            Interval = 2,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 15))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        var expectedTimes = new List<DateTime>
        {
            new (2024, 03, 04, 9, 0, 0),
            new (2024, 03, 04, 11, 0, 0),
            new (2024, 03, 04, 13, 0, 0),
            new (2024, 03, 04, 15, 0, 0),
            new (2024, 03, 04, 17, 0, 0),
            new (2024, 03, 06, 9, 0, 0),
            new (2024, 03, 06, 11, 0, 0),
            new (2024, 03, 06, 13, 0, 0),
            new (2024, 03, 06, 15, 0, 0),
            new (2024, 03, 06, 17, 0, 0),
            new (2024, 03, 11, 9, 0, 0),
            new (2024, 03, 11, 11, 0, 0)
        };

        executionTimes.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void DailyFrequencyConfiguration_ShouldRespectHourRange()
    {
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            Interval = 1,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 03))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        var expectedTimes = new List<DateTime>
        {
            new (2024, 03, 01, 9, 0, 0),
            new (2024, 03, 01, 10, 0, 0),
            new (2024, 03, 01, 11, 0, 0),
            new (2024, 03, 01, 12, 0, 0),
            new (2024, 03, 01, 13, 0, 0),
            new (2024, 03, 01, 14, 0, 0),
            new (2024, 03, 01, 15, 0, 0),
            new (2024, 03, 01, 16, 0, 0),
            new (2024, 03, 01, 17, 0, 0),
            new (2024, 03, 02, 9, 0, 0),
            new (2024, 03, 02, 10, 0, 0),
            new (2024, 03, 02, 11, 0, 0)
        };

        executionTimes.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void WeeklyFrequencyConfiguration_ShouldRespectHourRange()
    {
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
            WeekInterval = 1,
            Interval = 2,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 15))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        var expectedTimes = new List<DateTime>
        {
            new(2024, 03, 04, 9, 0, 0),
            new(2024, 03, 04, 11, 0, 0),
            new(2024, 03, 04, 13, 0, 0),
            new(2024, 03, 04, 15, 0, 0),
            new(2024, 03, 04, 17, 0, 0),
            new(2024, 03, 06, 9, 0, 0),
            new(2024, 03, 06, 11, 0, 0),
            new(2024, 03, 06, 13, 0, 0),
            new(2024, 03, 06, 15, 0, 0),
            new(2024, 03, 06, 17, 0, 0),
            new(2024, 03, 11, 9, 0, 0),
            new(2024, 03, 11, 11, 0, 0)
        };

        executionTimes.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void DailyFrequencyConfiguration_ShouldRespectHourlyInterval()
    {
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            Interval = 3,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(21, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 02))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        var expectedTimes = new List<DateTime>
        {
            new (2024, 03, 01, 9, 0, 0),
            new (2024, 03, 01, 12, 0, 0),
            new (2024, 03, 01, 15, 0, 0),
            new (2024, 03, 01, 18, 0, 0),
            new (2024, 03, 01, 21, 0, 0),
        };

        executionTimes.Should().BeEquivalentTo(expectedTimes);
    }

    [Fact]
    public void WeeklyFrequencyConfiguration_ShouldRespectHourlyInterval()
    {
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            DaysOfWeek = [DayOfWeek.Monday],
            WeekInterval = 1,
            Interval = 3,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(21, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 31))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        var expectedTimes = new List<DateTime>
        {
            new (2024, 03, 04, 9, 0, 0),
            new (2024, 03, 04, 12, 0, 0),
            new (2024, 03, 04, 15, 0, 0),
            new (2024, 03, 04, 18, 0, 0),
            new (2024, 03, 04, 21, 0, 0),
            new (2024, 03, 11, 9, 0, 0),
            new (2024, 03, 11, 12, 0, 0),
            new (2024, 03, 11, 15, 0, 0),
            new (2024, 03, 11, 18, 0, 0),
            new (2024, 03, 11, 21, 0, 0),
            new (2024, 03, 18, 9, 0, 0),
            new (2024, 03, 18, 12, 0, 0)
        };

        executionTimes.Should().BeEquivalentTo(expectedTimes);
    }
    [Fact]
    public void Configuration_WithCurrentDateInPast_ShouldHandleCorrectly()
    {
        var configuration = new OnceSchedulerConfiguration
        {
            CurrentDate = new DateTime(2023, 01, 01),
            IsEnabled = true,
            ConfigurationDateTime = new DateTime(2024, 01, 01),
            Limits = new LimitsTimeInterval(new DateTime(2023, 01, 01), new DateTime(2024, 01, 02))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        executionTimes.Should().ContainSingle()
    .Which.Should().Be(new DateTime(2024, 01, 01));
    }

    [Fact]
    public void Configuration_WithLimitDateInPast_ShouldHandleCorrectly()
    {
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2023, 01, 01), new DateTime(2023, 12, 31))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        executionTimes.Should().BeEmpty();
    }

    [Fact]
    public void Configuration_WithLimitDateInFuture_ShouldHandleCorrectly()
    {
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Interval = 2,
            IntervalType = IntervalType.Hourly,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 05))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        executionTimes.Should().HaveCount(4);
    }

    [Fact]
    public void Configuration_WithHourRangeCrossingMidnight_ShouldHandleCorrectly()
    {
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Interval = 2,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(22, 0, 0), new TimeSpan(2, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 02, 0, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        executionTimes.Should().HaveCount(2);
    }

    [Fact]
    public void Configuration_Null_ShouldThrowException()
    {
        SchedulerConfiguration configuration = null;

        Action act = () => _timeGenerator.GenerateExecutions(configuration, 12);

        act.Should().Throw<NullReferenceException>();
    }


    [Fact]
    public void Configuration_InvalidHourlyInterval_ShouldThrowException()
    {
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Interval = -1,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 02)),
            Culture = CultureOptions.en_GB
        };
        CultureInfo.CurrentCulture = new CultureInfo("en-GB");

        Action act = () => _timeGenerator.GenerateExecutions(configuration, 12);

        act.Should().Throw<ArgumentException>().WithMessage("Invalid interval");
    }


    [Fact]
    public void Configuration_WithLeapYear_ShouldHandleCorrectly()
    {
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 02, 28),
            IsEnabled = true,
            Interval = 24,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 02, 28), new DateTime(2024, 03, 01))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        executionTimes.Should().Contain(new DateTime(2024, 02, 29));
    }

    [Fact]
    public void Configuration_WithLeapYearInFebruary_ShouldHandleCorrectly()
    {
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 02, 28),
            IsEnabled = true,
            Interval = 24,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 02, 28), new DateTime(2024, 02, 29))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        executionTimes.Should().Contain(new DateTime(2024, 02, 29));
    }

    [Fact]
    public void Configuration_WithNonLeapYearInFebruary_ShouldHandleCorrectly()
    {
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2023, 02, 28),
            IsEnabled = true,
            Interval = 24,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59)),
            Limits = new LimitsTimeInterval(new DateTime(2023, 02, 28), new DateTime(2023, 03, 01))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        executionTimes.Should().Contain(new DateTime(2023, 02, 28));
        executionTimes.Should().Contain(new DateTime(2023, 03, 01));
    }
    [Fact]
    public void Configuration_WithComplexIntervals_ShouldHandleCorrectly()
    {
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
            WeekInterval = 2,
            Interval = 3,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(15, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 31))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        executionTimes.Should().HaveCount(12);
    }

    [Fact]
    public void Configuration_WithComplexHourRanges_ShouldHandleCorrectly()
    {
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            DaysOfWeek = [DayOfWeek.Monday],
            WeekInterval = 1,
            Interval = 2,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(21, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 15))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        executionTimes.Should().HaveCount(12);
    }


    [Fact]
    public void Configuration_WithWeekendExclusion_ShouldHandleCorrectly()
    {
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            DaysOfWeek =
        [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday],
            WeekInterval = 1,
            Interval = 1,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 31))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        executionTimes.Should().OnlyContain(dt => dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday);
    }

    [Fact]
    public void Configuration_WithWeekendOnly_ShouldHandleCorrectly()
    {
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            DaysOfWeek = [DayOfWeek.Saturday, DayOfWeek.Sunday],
            WeekInterval = 1,
            Interval = 1,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 31))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        executionTimes.Should().OnlyContain(dt => dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday);
    }

    [Fact]
    public void ValidateConfiguration_WithDateTimeInPast_ShouldReturnException()
    {
        var configuration = new OnceSchedulerConfiguration()
        {
            ConfigurationDateTime = new DateTime(2023, 01, 01),
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01))
        };

        Action act = () => _timeGenerator.GenerateExecutions(configuration, 12);


        act.Should().Throw<ArgumentException>("Configuration date time cannot be in the past.");

    }
    [Fact]
    public void ValidateConfiguration_WithNullOnceConfiguration_ShouldReturnException()
    {
        var invalidConfig = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Interval = 1,
            IntervalType = IntervalType.Hourly
        };

        Action act = () => new OnceDateCalculator().CalculateDates(invalidConfig, 12);


        act.Should().Throw<ArgumentException>("Invalid configuration type for OnceDateCalculator.");

    }

    [Fact]
    public void GenerateExecutions_ShouldThrowException_ForUnknownConfigurationType()
    {
        var unknownConfiguration = new UnsupportedConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
        };

        Action act = () => _timeGenerator.GenerateExecutions(unknownConfiguration, 12);

        act.Should().Throw<ArgumentException>().WithMessage("Unknown configuration type");
    }
    [Fact]
    public void MonthlyFrequency_WithAllFirstMondayInAYear_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.First,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.Monday,
            HourTimeRange = new HourTimeRange(new TimeSpan(13, 00, 00), new TimeSpan(13, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        executionTimes.Should().HaveCount(12);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 01, 13, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 02, 05, 13, 00, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 03, 04, 13, 00, 00));
        executionTimes[3].Should().Be(new DateTime(2024, 04, 01, 13, 00, 00));
        executionTimes[4].Should().Be(new DateTime(2024, 05, 06, 13, 00, 00));
        executionTimes[5].Should().Be(new DateTime(2024, 06, 03, 13, 00, 00));
        executionTimes[6].Should().Be(new DateTime(2024, 07, 01, 13, 00, 00));
        executionTimes[7].Should().Be(new DateTime(2024, 08, 05, 13, 00, 00));
        executionTimes[8].Should().Be(new DateTime(2024, 09, 02, 13, 00, 00));
        executionTimes[9].Should().Be(new DateTime(2024, 10, 07, 13, 00, 00));
        executionTimes[10].Should().Be(new DateTime(2024, 11, 04, 13, 00, 00));
        executionTimes[11].Should().Be(new DateTime(2024, 12, 02, 13, 00, 00));
    }

    [Fact]
    public void MonthlyFrequency_WithAllSecondWednesdayInAYear_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.Second,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.Wednesday,
            HourTimeRange = new HourTimeRange(new TimeSpan(4, 00, 00), new TimeSpan(4, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        executionTimes.Should().HaveCount(12);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 10, 4, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 02, 14, 4, 00, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 03, 13, 4, 00, 00));
        executionTimes[3].Should().Be(new DateTime(2024, 04, 10, 4, 00, 00));
        executionTimes[4].Should().Be(new DateTime(2024, 05, 08, 4, 00, 00));
        executionTimes[5].Should().Be(new DateTime(2024, 06, 12, 4, 00, 00));
        executionTimes[6].Should().Be(new DateTime(2024, 07, 10, 4, 00, 00));
        executionTimes[7].Should().Be(new DateTime(2024, 08, 14, 4, 00, 00));
        executionTimes[8].Should().Be(new DateTime(2024, 09, 11, 4, 00, 00));
        executionTimes[9].Should().Be(new DateTime(2024, 10, 09, 4, 00, 00));
        executionTimes[10].Should().Be(new DateTime(2024, 11, 13, 4, 00, 00));
        executionTimes[11].Should().Be(new DateTime(2024, 12, 11, 4, 00, 00));
    }

    [Fact]
    public void MonthlyFrequency_WithAllLastSaturdayInAYear_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.Last,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.Saturday,
            HourTimeRange = new HourTimeRange(new TimeSpan(21, 30, 00), new TimeSpan(21, 30, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        executionTimes.Should().HaveCount(12);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 27, 21, 30, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 02, 24, 21, 30, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 03, 30, 21, 30, 00));
        executionTimes[3].Should().Be(new DateTime(2024, 04, 27, 21, 30, 00));
        executionTimes[4].Should().Be(new DateTime(2024, 05, 25, 21, 30, 00));
        executionTimes[5].Should().Be(new DateTime(2024, 06, 29, 21, 30, 00));
        executionTimes[6].Should().Be(new DateTime(2024, 07, 27, 21, 30, 00));
        executionTimes[7].Should().Be(new DateTime(2024, 08, 31, 21, 30, 00));
        executionTimes[8].Should().Be(new DateTime(2024, 09, 28, 21, 30, 00));
        executionTimes[9].Should().Be(new DateTime(2024, 10, 26, 21, 30, 00));
        executionTimes[10].Should().Be(new DateTime(2024, 11, 30, 21, 30, 00));
        executionTimes[11].Should().Be(new DateTime(2024, 12, 28, 21, 30, 00));
    }
    [Fact]
    public void MonthlyFrequency_WithFirstWeekendOfMarch_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 03, 01),
            DayOptions = DayOptions.First,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 31)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.WeekendDay,
            HourTimeRange = new HourTimeRange(new TimeSpan(10, 00, 00), new TimeSpan(10, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 2);

        executionTimes.Should().HaveCount(2);

        executionTimes[0].Should().Be(new DateTime(2024, 03, 02, 10, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 03, 03, 10, 00, 00));
    }
    [Fact]
    public void MonthlyFrequency_WithFirstDayEveryTwoMonths_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.First,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 2,
            WeekOption = WeekOptions.AnyDay,
            HourTimeRange = new HourTimeRange(new TimeSpan(8, 00, 00), new TimeSpan(8, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        executionTimes.Should().HaveCount(6);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 01, 8, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 03, 01, 8, 00, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 05, 01, 8, 00, 00));
        executionTimes[3].Should().Be(new DateTime(2024, 07, 01, 8, 00, 00));
        executionTimes[4].Should().Be(new DateTime(2024, 09, 01, 8, 00, 00));
        executionTimes[5].Should().Be(new DateTime(2024, 11, 01, 8, 00, 00));
    }
    [Fact]
    public void MonthlyFrequency_WithFourthDayEveryFourMonths_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.First,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 4,
            WeekOption = WeekOptions.AnyDay,
            HourTimeRange = new HourTimeRange(new TimeSpan(8, 00, 00), new TimeSpan(8, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        executionTimes.Should().HaveCount(3);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 01, 8, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 05, 01, 8, 00, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 09, 01, 8, 00, 00));
    }
    [Fact]
    public void MonthlyFrequency_WithThirdWeekdayInAYear_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.Third,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.Weekday,
            HourTimeRange = new HourTimeRange(new TimeSpan(8, 00, 00), new TimeSpan(8, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        executionTimes.Should().HaveCount(12);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 03, 8, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 02, 05, 8, 00, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 03, 05, 8, 00, 00));
        executionTimes[3].Should().Be(new DateTime(2024, 04, 03, 8, 00, 00));
        executionTimes[4].Should().Be(new DateTime(2024, 05, 03, 8, 00, 00));
        executionTimes[5].Should().Be(new DateTime(2024, 06, 05, 8, 00, 00));
        executionTimes[6].Should().Be(new DateTime(2024, 07, 03, 8, 00, 00));
        executionTimes[7].Should().Be(new DateTime(2024, 08, 05, 8, 00, 00));
        executionTimes[8].Should().Be(new DateTime(2024, 09, 04, 8, 00, 00));
        executionTimes[9].Should().Be(new DateTime(2024, 10, 03, 8, 00, 00));
        executionTimes[10].Should().Be(new DateTime(2024, 11, 05, 8, 00, 00));
        executionTimes[11].Should().Be(new DateTime(2024, 12, 04, 8, 00, 00));
    }
    [Fact]
    public void MonthlyFrequency_WithSecondWeekendInAYear_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.Second,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.WeekendDay,
            HourTimeRange = new HourTimeRange(new TimeSpan(10, 00, 00), new TimeSpan(10, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        executionTimes.Should().HaveCount(12);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 13, 10, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 01, 14, 10, 00, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 02, 10, 10, 00, 00));
        executionTimes[3].Should().Be(new DateTime(2024, 02, 11, 10, 00, 00));
        executionTimes[4].Should().Be(new DateTime(2024, 03, 09, 10, 00, 00));
        executionTimes[5].Should().Be(new DateTime(2024, 03, 10, 10, 00, 00));
        executionTimes[6].Should().Be(new DateTime(2024, 04, 13, 10, 00, 00));
        executionTimes[7].Should().Be(new DateTime(2024, 04, 14, 10, 00, 00));
        executionTimes[8].Should().Be(new DateTime(2024, 05, 11, 10, 00, 00));
        executionTimes[9].Should().Be(new DateTime(2024, 05, 12, 10, 00, 00));
        executionTimes[10].Should().Be(new DateTime(2024, 06, 08, 10, 00, 00));
        executionTimes[11].Should().Be(new DateTime(2024, 06, 09, 10, 00, 00));
    }
    [Fact]
    public void MonthlyFrequency_WithThirdWeekendInAYear_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.Third,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.WeekendDay,
            HourTimeRange = new HourTimeRange(new TimeSpan(10, 00, 00), new TimeSpan(10, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        executionTimes.Should().HaveCount(12);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 20, 10, 00, 00)); executionTimes[1].Should().Be(new DateTime(2024, 01, 21, 10, 00, 00)); executionTimes[2].Should().Be(new DateTime(2024, 02, 17, 10, 00, 00)); executionTimes[3].Should().Be(new DateTime(2024, 02, 18, 10, 00, 00)); executionTimes[4].Should().Be(new DateTime(2024, 03, 16, 10, 00, 00)); executionTimes[5].Should().Be(new DateTime(2024, 03, 17, 10, 00, 00)); executionTimes[6].Should().Be(new DateTime(2024, 04, 20, 10, 00, 00)); executionTimes[7].Should().Be(new DateTime(2024, 04, 21, 10, 00, 00)); executionTimes[8].Should().Be(new DateTime(2024, 05, 18, 10, 00, 00)); executionTimes[9].Should().Be(new DateTime(2024, 05, 19, 10, 00, 00)); executionTimes[10].Should().Be(new DateTime(2024, 06, 15, 10, 00, 00)); executionTimes[11].Should().Be(new DateTime(2024, 06, 16, 10, 00, 00));
    }

    [Fact]
    public void MonthlyFrequency_WithLastWeekdayInAYear_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.Last,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2025, 01, 01)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.Weekday,
            HourTimeRange = new HourTimeRange(new TimeSpan(17, 00, 00), new TimeSpan(17, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        executionTimes.Should().HaveCount(12);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 31, 17, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 02, 29, 17, 00, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 03, 29, 17, 00, 00));
        executionTimes[3].Should().Be(new DateTime(2024, 04, 30, 17, 00, 00));
        executionTimes[4].Should().Be(new DateTime(2024, 05, 31, 17, 00, 00));
        executionTimes[5].Should().Be(new DateTime(2024, 06, 28, 17, 00, 00));
        executionTimes[6].Should().Be(new DateTime(2024, 07, 31, 17, 00, 00));
        executionTimes[7].Should().Be(new DateTime(2024, 08, 30, 17, 00, 00));
        executionTimes[8].Should().Be(new DateTime(2024, 09, 30, 17, 00, 00));
        executionTimes[9].Should().Be(new DateTime(2024, 10, 31, 17, 00, 00));
        executionTimes[10].Should().Be(new DateTime(2024, 11, 29, 17, 00, 00));
        executionTimes[11].Should().Be(new DateTime(2024, 12, 31, 17, 00, 00));
    }
    [Fact]
    public void MonthlyFrequency_WithLastSaturdaySixMonths_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 07, 01),
            DayOptions = DayOptions.Last,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 07, 01), new DateTime(2025, 06, 30)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.Saturday,
            HourTimeRange = new HourTimeRange(new TimeSpan(21, 30, 00), new TimeSpan(21, 30, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        executionTimes.Should().HaveCount(12);

        executionTimes[0].Should().Be(new DateTime(2024, 07, 27, 21, 30, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 08, 31, 21, 30, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 09, 28, 21, 30, 00));
        executionTimes[3].Should().Be(new DateTime(2024, 10, 26, 21, 30, 00));
        executionTimes[4].Should().Be(new DateTime(2024, 11, 30, 21, 30, 00));
        executionTimes[5].Should().Be(new DateTime(2024, 12, 28, 21, 30, 00));
        executionTimes[6].Should().Be(new DateTime(2025, 01, 25, 21, 30, 00));
        executionTimes[7].Should().Be(new DateTime(2025, 02, 22, 21, 30, 00));
        executionTimes[8].Should().Be(new DateTime(2025, 03, 29, 21, 30, 00));
        executionTimes[9].Should().Be(new DateTime(2025, 04, 26, 21, 30, 00));
        executionTimes[10].Should().Be(new DateTime(2025, 05, 31, 21, 30, 00));
        executionTimes[11].Should().Be(new DateTime(2025, 06, 28, 21, 30, 00));
    }

    [Fact]
    public void MonthlyFrequency_WithAllThirdFridayInAYear_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.Third,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.Friday,
            HourTimeRange = new HourTimeRange(new TimeSpan(15, 00, 00), new TimeSpan(15, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        executionTimes.Should().HaveCount(12);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 19, 15, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 02, 16, 15, 00, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 03, 15, 15, 00, 00));
        executionTimes[3].Should().Be(new DateTime(2024, 04, 19, 15, 00, 00));
        executionTimes[4].Should().Be(new DateTime(2024, 05, 17, 15, 00, 00));
        executionTimes[5].Should().Be(new DateTime(2024, 06, 21, 15, 00, 00));
        executionTimes[6].Should().Be(new DateTime(2024, 07, 19, 15, 00, 00));
        executionTimes[7].Should().Be(new DateTime(2024, 08, 16, 15, 00, 00));
        executionTimes[8].Should().Be(new DateTime(2024, 09, 20, 15, 00, 00));
        executionTimes[9].Should().Be(new DateTime(2024, 10, 18, 15, 00, 00));
        executionTimes[10].Should().Be(new DateTime(2024, 11, 15, 15, 00, 00));
        executionTimes[11].Should().Be(new DateTime(2024, 12, 20, 15, 00, 00));
    }

    [Fact]
    public void MonthlyFrequency_WithSpecificDayEveryThreeMonths_ShouldReturnCorrectly()
    {
        var config = new SpecificDayMonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            SpecificDay = 15,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2025, 01, 01)),
            MonthFrequency = 3,
            HourTimeRange = new HourTimeRange(new TimeSpan(12, 00, 00), new TimeSpan(12, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 4);

        executionTimes.Should().HaveCount(4);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 15, 12, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 04, 15, 12, 00, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 07, 15, 12, 00, 00));
        executionTimes[3].Should().Be(new DateTime(2024, 10, 15, 12, 00, 00));
    }
    [Fact]
    public void MonthlyFrequency_WithFourthWednesdayInEveryTwoMonths_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.Fourth,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 2,
            WeekOption = WeekOptions.Wednesday,
            HourTimeRange = new HourTimeRange(new TimeSpan(11, 00, 00), new TimeSpan(11, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 6);

        executionTimes.Should().HaveCount(6);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 24, 11, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 03, 27, 11, 00, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 05, 22, 11, 00, 00));
        executionTimes[3].Should().Be(new DateTime(2024, 07, 24, 11, 00, 00));
        executionTimes[4].Should().Be(new DateTime(2024, 09, 25, 11, 00, 00));
        executionTimes[5].Should().Be(new DateTime(2024, 11, 27, 11, 00, 00));
    }
    [Fact]
    public void MonthlyFrequency_WithLastWeekendInAYear_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.Last,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.WeekendDay,
            HourTimeRange = new HourTimeRange(new TimeSpan(10, 00, 00), new TimeSpan(10, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 24);

        executionTimes.Should().HaveCount(24);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 27, 10, 00, 00)); executionTimes[1].Should().Be(new DateTime(2024, 01, 28, 10, 00, 00)); executionTimes[2].Should().Be(new DateTime(2024, 02, 24, 10, 00, 00)); executionTimes[3].Should().Be(new DateTime(2024, 02, 25, 10, 00, 00)); executionTimes[4].Should().Be(new DateTime(2024, 03, 30, 10, 00, 00)); executionTimes[5].Should().Be(new DateTime(2024, 03, 31, 10, 00, 00)); executionTimes[6].Should().Be(new DateTime(2024, 04, 27, 10, 00, 00)); executionTimes[7].Should().Be(new DateTime(2024, 04, 28, 10, 00, 00)); executionTimes[8].Should().Be(new DateTime(2024, 05, 25, 10, 00, 00)); executionTimes[9].Should().Be(new DateTime(2024, 05, 26, 10, 00, 00)); executionTimes[10].Should().Be(new DateTime(2024, 06, 29, 10, 00, 00)); executionTimes[11].Should().Be(new DateTime(2024, 06, 30, 10, 00, 00)); executionTimes[12].Should().Be(new DateTime(2024, 07, 27, 10, 00, 00)); executionTimes[13].Should().Be(new DateTime(2024, 07, 28, 10, 00, 00)); executionTimes[14].Should().Be(new DateTime(2024, 08, 31, 10, 00, 00)); executionTimes[15].Should().Be(new DateTime(2024, 09, 01, 10, 00, 00)); executionTimes[16].Should().Be(new DateTime(2024, 09, 28, 10, 00, 00)); executionTimes[17].Should().Be(new DateTime(2024, 09, 29, 10, 00, 00)); executionTimes[18].Should().Be(new DateTime(2024, 10, 26, 10, 00, 00)); executionTimes[19].Should().Be(new DateTime(2024, 10, 27, 10, 00, 00)); executionTimes[20].Should().Be(new DateTime(2024, 11, 30, 10, 00, 00)); executionTimes[21].Should().Be(new DateTime(2024, 12, 01, 10, 00, 00)); executionTimes[22].Should().Be(new DateTime(2024, 12, 28, 10, 00, 00)); executionTimes[23].Should().Be(new DateTime(2024, 12, 29, 10, 00, 00));
    }
    [Fact]
    public void MonthlyFrequency_WithAnyDay_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.First,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.AnyDay,
            HourTimeRange = new HourTimeRange(new TimeSpan(8, 00, 00), new TimeSpan(8, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        executionTimes.Should().HaveCount(12);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 01, 8, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 02, 01, 8, 00, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 03, 01, 8, 00, 00));
        executionTimes[3].Should().Be(new DateTime(2024, 04, 01, 8, 00, 00));
        executionTimes[4].Should().Be(new DateTime(2024, 05, 01, 8, 00, 00));
        executionTimes[5].Should().Be(new DateTime(2024, 06, 01, 8, 00, 00));
        executionTimes[6].Should().Be(new DateTime(2024, 07, 01, 8, 00, 00));
        executionTimes[7].Should().Be(new DateTime(2024, 08, 01, 8, 00, 00));
        executionTimes[8].Should().Be(new DateTime(2024, 09, 01, 8, 00, 00));
        executionTimes[9].Should().Be(new DateTime(2024, 10, 01, 8, 00, 00));
        executionTimes[10].Should().Be(new DateTime(2024, 11, 01, 8, 00, 00));
        executionTimes[11].Should().Be(new DateTime(2024, 12, 01, 8, 00, 00));
    }
    [Fact]
    public void MonthlyFrequency_WithLastWeekdayWeekend_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.Last,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.WeekendDay,
            HourTimeRange = new HourTimeRange(new TimeSpan(10, 00, 00), new TimeSpan(10, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 24);

        executionTimes.Should().HaveCount(24);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 27, 10, 00, 00)); executionTimes[1].Should().Be(new DateTime(2024, 01, 28, 10, 00, 00)); executionTimes[2].Should().Be(new DateTime(2024, 02, 24, 10, 00, 00)); executionTimes[3].Should().Be(new DateTime(2024, 02, 25, 10, 00, 00)); executionTimes[4].Should().Be(new DateTime(2024, 03, 30, 10, 00, 00)); executionTimes[5].Should().Be(new DateTime(2024, 03, 31, 10, 00, 00)); executionTimes[6].Should().Be(new DateTime(2024, 04, 27, 10, 00, 00)); executionTimes[7].Should().Be(new DateTime(2024, 04, 28, 10, 00, 00)); executionTimes[8].Should().Be(new DateTime(2024, 05, 25, 10, 00, 00)); executionTimes[9].Should().Be(new DateTime(2024, 05, 26, 10, 00, 00)); executionTimes[10].Should().Be(new DateTime(2024, 06, 29, 10, 00, 00)); executionTimes[11].Should().Be(new DateTime(2024, 06, 30, 10, 00, 00)); executionTimes[12].Should().Be(new DateTime(2024, 07, 27, 10, 00, 00)); executionTimes[13].Should().Be(new DateTime(2024, 07, 28, 10, 00, 00)); executionTimes[14].Should().Be(new DateTime(2024, 08, 31, 10, 00, 00)); executionTimes[15].Should().Be(new DateTime(2024, 09, 01, 10, 00, 00)); executionTimes[16].Should().Be(new DateTime(2024, 09, 28, 10, 00, 00)); executionTimes[17].Should().Be(new DateTime(2024, 09, 29, 10, 00, 00)); executionTimes[18].Should().Be(new DateTime(2024, 10, 26, 10, 00, 00)); executionTimes[19].Should().Be(new DateTime(2024, 10, 27, 10, 00, 00)); executionTimes[20].Should().Be(new DateTime(2024, 11, 30, 10, 00, 00)); executionTimes[21].Should().Be(new DateTime(2024, 12, 01, 10, 00, 00)); executionTimes[22].Should().Be(new DateTime(2024, 12, 28, 10, 00, 00)); executionTimes[23].Should().Be(new DateTime(2024, 12, 29, 10, 00, 00));
    }
    [Fact]
    public void MonthlyFrequency_WithSecondWeekendOfAYear_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.Second,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.WeekendDay,
            HourTimeRange = new HourTimeRange(new TimeSpan(10, 00, 00), new TimeSpan(10, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 24);

        executionTimes.Should().HaveCount(24);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 13, 10, 00, 00)); executionTimes[1].Should().Be(new DateTime(2024, 01, 14, 10, 00, 00)); executionTimes[2].Should().Be(new DateTime(2024, 02, 10, 10, 00, 00)); executionTimes[3].Should().Be(new DateTime(2024, 02, 11, 10, 00, 00)); executionTimes[4].Should().Be(new DateTime(2024, 03, 09, 10, 00, 00)); executionTimes[5].Should().Be(new DateTime(2024, 03, 10, 10, 00, 00)); executionTimes[6].Should().Be(new DateTime(2024, 04, 13, 10, 00, 00)); executionTimes[7].Should().Be(new DateTime(2024, 04, 14, 10, 00, 00)); executionTimes[8].Should().Be(new DateTime(2024, 05, 11, 10, 00, 00)); executionTimes[9].Should().Be(new DateTime(2024, 05, 12, 10, 00, 00)); executionTimes[10].Should().Be(new DateTime(2024, 06, 08, 10, 00, 00)); executionTimes[11].Should().Be(new DateTime(2024, 06, 09, 10, 00, 00)); executionTimes[12].Should().Be(new DateTime(2024, 07, 13, 10, 00, 00)); executionTimes[13].Should().Be(new DateTime(2024, 07, 14, 10, 00, 00)); executionTimes[14].Should().Be(new DateTime(2024, 08, 10, 10, 00, 00)); executionTimes[15].Should().Be(new DateTime(2024, 08, 11, 10, 00, 00)); executionTimes[16].Should().Be(new DateTime(2024, 09, 07, 10, 00, 00)); executionTimes[17].Should().Be(new DateTime(2024, 09, 08, 10, 00, 00)); executionTimes[18].Should().Be(new DateTime(2024, 10, 12, 10, 00, 00)); executionTimes[19].Should().Be(new DateTime(2024, 10, 13, 10, 00, 00)); executionTimes[20].Should().Be(new DateTime(2024, 11, 09, 10, 00, 00)); executionTimes[21].Should().Be(new DateTime(2024, 11, 10, 10, 00, 00)); executionTimes[22].Should().Be(new DateTime(2024, 12, 07, 10, 00, 00)); executionTimes[23].Should().Be(new DateTime(2024, 12, 08, 10, 00, 00));
    }
    [Fact]
    public void MonthlyFrequency_WithFirstWeekendSunday_ShouldReturnCorrectly()
    {
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.First,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.WeekendDay,
            HourTimeRange = new HourTimeRange(new TimeSpan(10, 00, 00), new TimeSpan(10, 00, 00))
        };

        var executionTimes = _timeGenerator.GenerateExecutions(config, 24);

        executionTimes.Should().HaveCount(24);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 06, 10, 00, 00)); executionTimes[1].Should().Be(new DateTime(2024, 01, 07, 10, 00, 00)); executionTimes[2].Should().Be(new DateTime(2024, 02, 03, 10, 00, 00)); executionTimes[3].Should().Be(new DateTime(2024, 02, 04, 10, 00, 00)); executionTimes[4].Should().Be(new DateTime(2024, 03, 02, 10, 00, 00)); executionTimes[5].Should().Be(new DateTime(2024, 03, 03, 10, 00, 00)); executionTimes[6].Should().Be(new DateTime(2024, 04, 06, 10, 00, 00)); executionTimes[7].Should().Be(new DateTime(2024, 04, 07, 10, 00, 00)); executionTimes[8].Should().Be(new DateTime(2024, 05, 04, 10, 00, 00)); executionTimes[9].Should().Be(new DateTime(2024, 05, 05, 10, 00, 00)); executionTimes[10].Should().Be(new DateTime(2024, 06, 01, 10, 00, 00)); executionTimes[11].Should().Be(new DateTime(2024, 06, 02, 10, 00, 00)); executionTimes[12].Should().Be(new DateTime(2024, 07, 06, 10, 00, 00)); executionTimes[13].Should().Be(new DateTime(2024, 07, 07, 10, 00, 00)); executionTimes[14].Should().Be(new DateTime(2024, 08, 03, 10, 00, 00)); executionTimes[15].Should().Be(new DateTime(2024, 08, 04, 10, 00, 00)); executionTimes[16].Should().Be(new DateTime(2024, 08, 31, 10, 00, 00)); executionTimes[17].Should().Be(new DateTime(2024, 09, 01, 10, 00, 00)); executionTimes[18].Should().Be(new DateTime(2024, 10, 05, 10, 00, 00)); executionTimes[19].Should().Be(new DateTime(2024, 10, 06, 10, 00, 00)); executionTimes[20].Should().Be(new DateTime(2024, 11, 02, 10, 00, 00)); executionTimes[21].Should().Be(new DateTime(2024, 11, 03, 10, 00, 00)); executionTimes[22].Should().Be(new DateTime(2024, 11, 30, 10, 00, 00)); executionTimes[23].Should().Be(new DateTime(2024, 12, 01, 10, 00, 00));
    }

 
    [Fact]
    public void CalculateDates_WithInvalidConfigurationTypeForSpecificDay_ShouldThrowArgumentException()
    {
        var strategy = new SpecificDayStrategy();
        var config = new MonthlySchedulerConfiguration();
        Action act = () => strategy.CalculateDates(config, 12);

        act.Should().Throw<ArgumentException>().WithMessage("Invalid configuration type for SpecificDayStrategy.");
    }
    [Fact]
    public void CalculateDates_WithInvalidDayOptions_ShouldThrowArgumentOutOfRangeException()
    {
        var calculator = new MonthlyDateCalculator();
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = DateTime.Now,
            DayOptions = (DayOptions)999,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(DateTime.Now, DateTime.Now.AddYears(1)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.AnyDay,
            HourTimeRange = new HourTimeRange(new TimeSpan(8, 00, 00), new TimeSpan(8, 00, 00))
        };

        Action act = () => calculator.CalculateDates(config, 12);

        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*999*");
    }
    [Fact]
    public void MonthlyFrequency_WithSpecificDayOnLeapYear_ShouldReturnCorrectly()
    {
        // Arrange
        var config = new SpecificDayMonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            SpecificDay = 29,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(10, 00, 00), new TimeSpan(10, 00, 00))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        // Assert
        executionTimes.Should().HaveCount(12);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 29, 10, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 02, 29, 10, 00, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 03, 29, 10, 00, 00));
        executionTimes[3].Should().Be(new DateTime(2024, 04, 29, 10, 00, 00));
        executionTimes[4].Should().Be(new DateTime(2024, 05, 29, 10, 00, 00));
        executionTimes[5].Should().Be(new DateTime(2024, 06, 29, 10, 00, 00));
        executionTimes[6].Should().Be(new DateTime(2024, 07, 29, 10, 00, 00));
        executionTimes[7].Should().Be(new DateTime(2024, 08, 29, 10, 00, 00));
        executionTimes[8].Should().Be(new DateTime(2024, 09, 29, 10, 00, 00));
        executionTimes[9].Should().Be(new DateTime(2024, 10, 29, 10, 00, 00));
        executionTimes[10].Should().Be(new DateTime(2024, 11, 29, 10, 00, 00));
        executionTimes[11].Should().Be(new DateTime(2024, 12, 29, 10, 00, 00));
    }
    [Fact]
    public void MonthlyFrequency_WithDifferentHourTimeRanges_ShouldReturnCorrectly()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.First,
            Interval = 2,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.AnyDay,
            HourTimeRange = new HourTimeRange(new TimeSpan(8, 00, 00), new TimeSpan(18, 00, 00))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        // Assert
        executionTimes.Should().HaveCount(12);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 01, 8, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 01, 01, 10, 00, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 01, 01, 12, 00, 00));
        executionTimes[3].Should().Be(new DateTime(2024, 01, 01, 14, 00, 00));
        executionTimes[4].Should().Be(new DateTime(2024, 01, 01, 16, 00, 00));
        executionTimes[5].Should().Be(new DateTime(2024, 01, 01, 18, 00, 00));
    }
    [Fact]
    public void MonthlyFrequency_WithLimitsCrossingYears_ShouldReturnCorrectly()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2023, 12, 01),
            DayOptions = DayOptions.First,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2023, 12, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.AnyDay,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 00, 00), new TimeSpan(9, 00, 00))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(config, 13);

        // Assert
        executionTimes.Should().HaveCount(13);

        executionTimes[0].Should().Be(new DateTime(2023, 12, 01, 9, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 01, 01, 9, 00, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 02, 01, 9, 00, 00));
        executionTimes[3].Should().Be(new DateTime(2024, 03, 01, 9, 00, 00));
        executionTimes[4].Should().Be(new DateTime(2024, 04, 01, 9, 00, 00));
        executionTimes[5].Should().Be(new DateTime(2024, 05, 01, 9, 00, 00));
        executionTimes[6].Should().Be(new DateTime(2024, 06, 01, 9, 00, 00));
        executionTimes[7].Should().Be(new DateTime(2024, 07, 01, 9, 00, 00));
        executionTimes[8].Should().Be(new DateTime(2024, 08, 01, 9, 00, 00));
        executionTimes[9].Should().Be(new DateTime(2024, 09, 01, 9, 00, 00));
        executionTimes[10].Should().Be(new DateTime(2024, 10, 01, 9, 00, 00));
        executionTimes[11].Should().Be(new DateTime(2024, 11, 01, 9, 00, 00));
        executionTimes[12].Should().Be(new DateTime(2024, 12, 01, 9, 00, 00));
    }

    [Fact]
    public void GenerateDescription_ShouldThrowArgumentException_ForUnknownConfiguration()
    {
        // Arrange
        var configuration = new UnsupportedConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            Culture = CultureOptions.en_GB
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        Action act = () => _descriptionService.GenerateDescription(configuration, executionTime);
        CustomStringLocalizer localizer = new CustomStringLocalizer();

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage(localizer["ExecutionTimeGeneratorUnknownConfigurationExc", configuration.Culture].Value, "*");
    }

    [Fact]
    public void MonthlyFrequency_WithLimitsCrossingYears_ShouldReturnCorrectly_EnUs()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2023, 12, 01),
            DayOptions = DayOptions.First,
            Interval = 1,
            IntervalType = IntervalType.Hourly,
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2023, 12, 01), new DateTime(2024, 12, 31)),
            MonthFrequency = 1,
            WeekOption = WeekOptions.AnyDay,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 00, 00), new TimeSpan(9, 00, 00)),
            Culture = CultureOptions.en_US
        };

        var executionTime = new DateTime(2023, 12, 01, 9, 00, 00);
        var descriptionService = new DescriptionService(localizer);

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(config, 13);

        // Assert
        executionTimes.Should().HaveCount(13);

        executionTimes[0].Should().Be(new DateTime(2023, 12, 01, 9, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 01, 01, 9, 00, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 02, 01, 9, 00, 00));
        executionTimes[3].Should().Be(new DateTime(2024, 03, 01, 9, 00, 00));
        executionTimes[4].Should().Be(new DateTime(2024, 04, 01, 9, 00, 00));
        executionTimes[5].Should().Be(new DateTime(2024, 05, 01, 9, 00, 00));
        executionTimes[6].Should().Be(new DateTime(2024, 06, 01, 9, 00, 00));
        executionTimes[7].Should().Be(new DateTime(2024, 07, 01, 9, 00, 00));
        executionTimes[8].Should().Be(new DateTime(2024, 08, 01, 9, 00, 00));
        executionTimes[9].Should().Be(new DateTime(2024, 09, 01, 9, 00, 00));
        executionTimes[10].Should().Be(new DateTime(2024, 10, 01, 9, 00, 00));
        executionTimes[11].Should().Be(new DateTime(2024, 11, 01, 9, 00, 00));
        executionTimes[12].Should().Be(new DateTime(2024, 12, 01, 9, 00, 00));

        var description = descriptionService.GenerateDescription(config, executionTime);
        description.Should().Be("Occurs on day 1 of every 1 month(s). Schedule will be used on 12/1/2023 at 9:00 AM starting on 12/1/2023.");
    }
    [Fact]
    public void MonthlyFrequency_WithLimitsCrossingYears_ShouldReturnCorrectly_AnyDay()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2023, 12, 01),
            DayOptions = DayOptions.First,
            MonthFrequency = 1,
            WeekOption = WeekOptions.AnyDay,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 00, 00), new TimeSpan(9, 00, 00)),
            Limits = new LimitsTimeInterval(new DateTime(2023, 12, 01), new DateTime(2024, 12, 31)),
            Culture = CultureOptions.en_US
        };

        var executionTimes = new List<DateTime>
        {
            new DateTime(2023, 12, 01, 9, 00, 00),
            new DateTime(2024, 01, 01, 9, 00, 00),
            new DateTime(2024, 02, 01, 9, 00, 00),
            new DateTime(2024, 03, 01, 9, 00, 00),
            new DateTime(2024, 04, 01, 9, 00, 00),
            new DateTime(2024, 05, 01, 9, 00, 00),
            new DateTime(2024, 06, 01, 9, 00, 00),
            new DateTime(2024, 07, 01, 9, 00, 00),
            new DateTime(2024, 08, 01, 9, 00, 00),
            new DateTime(2024, 09, 01, 9, 00, 00),
            new DateTime(2024, 10, 01, 9, 00, 00),
            new DateTime(2024, 11, 01, 9, 00, 00),
            new DateTime(2024, 12, 01, 9, 00, 00)
        };

        // Act
        var result = _descriptionService.GenerateDescription(config, executionTimes[0]);

        // Assert
        result.Should().Be("Occurs on day 1 of every 1 month(s). Schedule will be used on 12/1/2023 at 9:00 AM starting on 12/1/2023.");
    }

    [Fact]
    public void MonthlyFrequency_WithLimitsCrossingYears_ShouldReturnCorrectly_Tuesday()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2023, 12, 01),
            DayOptions = DayOptions.First,
            MonthFrequency = 1,
            WeekOption = WeekOptions.Tuesday,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 00, 00), new TimeSpan(9, 00, 00)),
            Limits = new LimitsTimeInterval(new DateTime(2023, 12, 01), new DateTime(2024, 12, 31)),
            Culture = CultureOptions.en_US
        };

        var executionTimes = new List<DateTime>
        {
            new DateTime(2023, 12, 05, 9, 00, 00),
            new DateTime(2024, 01, 02, 9, 00, 00),
            new DateTime(2024, 02, 06, 9, 00, 00),
            new DateTime(2024, 03, 05, 9, 00, 00),
            new DateTime(2024, 04, 02, 9, 00, 00),
            new DateTime(2024, 05, 07, 9, 00, 00),
            new DateTime(2024, 06, 04, 9, 00, 00),
            new DateTime(2024, 07, 02, 9, 00, 00),
            new DateTime(2024, 08, 06, 9, 00, 00),
            new DateTime(2024, 09, 03, 9, 00, 00),
            new DateTime(2024, 10, 01, 9, 00, 00),
            new DateTime(2024, 11, 05, 9, 00, 00),
            new DateTime(2024, 12, 03, 9, 00, 00)
        };

        // Act
        var result = _descriptionService.GenerateDescription(config, executionTimes[0]);

        // Assert
        result.Should().Be("Occurs on the first Tuesday of every 1 month(s). Schedule will be used on 12/5/2023 at 9:00 AM starting on 12/1/2023.");
    }

    [Fact]
    public void MonthlyFrequency_WithLimitsCrossingYears_ShouldReturnCorrectly_Thursday()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2023, 12, 01),
            DayOptions = DayOptions.First,
            MonthFrequency = 1,
            WeekOption = WeekOptions.Thursday,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 00, 00), new TimeSpan(9, 00, 00)),
            Limits = new LimitsTimeInterval(new DateTime(2023, 12, 01), new DateTime(2024, 12, 31)),
            Culture = CultureOptions.en_US
        };

        var executionTimes = new List<DateTime>
        {
            new DateTime(2023, 12, 07, 9, 00, 00),
            new DateTime(2024, 01, 04, 9, 00, 00),
            new DateTime(2024, 02, 01, 9, 00, 00),
            new DateTime(2024, 03, 07, 9, 00, 00),
            new DateTime(2024, 04, 04, 9, 00, 00),
            new DateTime(2024, 05, 02, 9, 00, 00),
            new DateTime(2024, 06, 06, 9, 00, 00),
            new DateTime(2024, 07, 04, 9, 00, 00),
            new DateTime(2024, 08, 01, 9, 00, 00),
            new DateTime(2024, 09, 05, 9, 00, 00),
            new DateTime(2024, 10, 03, 9, 00, 00),
            new DateTime(2024, 11, 07, 9, 00, 00),
            new DateTime(2024, 12, 05, 9, 00, 00)
        };

        // Act
        var result = _descriptionService.GenerateDescription(config, executionTimes[0]);

        // Assert
        result.Should().Be("Occurs on the first Thursday of every 1 month(s). Schedule will be used on 12/7/2023 at 9:00 AM starting on 12/1/2023.");
    }

    [Fact]
    public void MonthlyFrequency_WithLimitsCrossingYears_ShouldReturnCorrectly_Sunday()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration()
        {
            CurrentDate = new DateTime(2023, 12, 01),
            DayOptions = DayOptions.First,
            MonthFrequency = 1,
            WeekOption = WeekOptions.Sunday,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 00, 00), new TimeSpan(9, 00, 00)),
            Limits = new LimitsTimeInterval(new DateTime(2023, 12, 01), new DateTime(2024, 12, 31)),
            Culture = CultureOptions.en_US
        };

        var executionTimes = new List<DateTime>
        {
            new DateTime(2023, 12, 03, 9, 00, 00),
            new DateTime(2024, 01, 07, 9, 00, 00),
            new DateTime(2024, 02, 04, 9, 00, 00),
            new DateTime(2024, 03, 03, 9, 00, 00),
            new DateTime(2024, 04, 07, 9, 00, 00),
            new DateTime(2024, 05, 05, 9, 00, 00),
            new DateTime(2024, 06, 02, 9, 00, 00),
            new DateTime(2024, 07, 07, 9, 00, 00),
            new DateTime(2024, 08, 04, 9, 00, 00),
            new DateTime(2024, 09, 01, 9, 00, 00),
            new DateTime(2024, 10, 06, 9, 00, 00),
            new DateTime(2024, 11, 03, 9, 00, 00),
            new DateTime(2024, 12, 01, 9, 00, 00)
        };

        // Act
        var result = _descriptionService.GenerateDescription(config, executionTimes[0]);

        // Assert
        result.Should().Be("Occurs on the first Sunday of every 1 month(s). Schedule will be used on 12/3/2023 at 9:00 AM starting on 12/1/2023.");
    }

    [Fact]
    public void LocalizedString_ShouldReturnKey_WhenFormatIsNull()
    {
        // Arrange
        string key = "NonExistentKey";


        CustomStringLocalizer _localizer = new CustomStringLocalizer();

        // Act
        var result = _localizer[key];

        // Assert
        Assert.Equal(key, result.Value);
        Assert.True(result.ResourceNotFound);
    }
    [Fact]
    public void GetAllStrings_ShouldReturnAllStringsForCulture()
    {
        // Arrange
        CultureInfo.CurrentCulture = new CultureInfo("en-GB");

        // Act
        CustomStringLocalizer _localizer = new CustomStringLocalizer();

        var result = _localizer.GetAllStrings(false).ToList();

        // Assert
        Assert.NotEmpty(result);
        Assert.All(result, localizedString => Assert.False(localizedString.ResourceNotFound));

        // Example assertions for specific keys
        Assert.Contains(result, ls => ls.Name == "DailyDescriptionSingle" && ls.Value == "Occurs once at {0}. Schedule will be used on {1} at {2} starting on {3}.");
        Assert.Contains(result, ls => ls.Name == "Monday" && ls.Value == "Monday");
    }

    [Fact]
    public void CalculateDates_ShouldThrowArgumentException_WhenConfigIsNotMonthlySchedulerConfiguration()
    {
        // Arrange
        var calculator = new MonthlyDateCalculator();
        var invalidConfig = new OnceSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            ConfigurationDateTime = new DateTime(2024, 01, 01, 9, 0, 0)
        };

        // Act
        Action act = () => calculator.CalculateDates(invalidConfig, 5);

        // Assert
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Equal("Invalid configuration type for MonthlyDateCalculator.", exception.Message);
    }
}



public class UnsupportedConfiguration : SchedulerConfiguration;

