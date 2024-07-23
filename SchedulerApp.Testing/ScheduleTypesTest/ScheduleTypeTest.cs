using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Interfaces;
using SchedulerApplication.Models;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Models.ValueObjects;
using SchedulerApplication.Services.DateCalculator;
using SchedulerApplication.Services.Description;
using SchedulerApplication.Services.ExecutionTime;
using SchedulerApplication.Services.HourCalculator;
using SchedulerApplication.Services.ScheduleTypes;

namespace SchedulerApp.Testing.ScheduleTypesTest;

public class ScheduleTypeTest
{
    private readonly ScheduleTypeFactory _factory;
    private readonly IDescriptionService _descriptionService;
    private readonly IExecutionTimeGenerator _timeGenerator;

    public ScheduleTypeTest()
    {
        _descriptionService = new DescriptionService();
        _timeGenerator = new ExecutionTimeGenerator(
            new OnceDateCalculator(),
            new DailyDateCalculator(),
            new WeeklyDateCalculator(),
            new HourCalculatorService(),
            new MonthlyDateCalculator());
        _factory = new ScheduleTypeFactory(_descriptionService, _timeGenerator);
    }

    [Fact]
    public void CreateSchedule_ShouldReturnCorrectExecution_WhenOnceSchedulerConfigurationIsCorrect()
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            ConfigurationDateTime = new DateTime(2024, 07, 15),
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 02, 09, 00, 00),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31))
        };

        // Act
        var scheduleType = _factory.CreateScheduleType(configuration,12);
        var executionTimes = scheduleType.GetNextExecutionTimes(configuration);

        // Assert
        scheduleType.Should().BeOfType<ScheduleTypeOnce>();
        executionTimes.Should().HaveCount(1);
        executionTimes[0].ExecutionTime.Should().Be(new DateTime(2024, 07, 15));
        executionTimes[0].Description.Should().Be("Occurs once. Schedule will be used on 15/07/2024 at 00:00 starting on 02/01/2024.");
    }

    [Fact]
    public void CreateSchedule_ShouldReturnCorrectExecution_WhenDailyFrequencyConfigurationIsCorrect()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 07, 15, 09, 02, 21),
            Interval = 2,
            IntervalType = IntervalType.Hourly,
            HourTimeRange = new HourTimeRange(new TimeSpan(21, 00, 00), new TimeSpan(23, 00, 00)),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 07, 15), new DateTime(2024, 07, 17))
        };

        // Act
        var scheduleType = _factory.CreateScheduleType(configuration,12);
        var executionTimes = scheduleType.GetNextExecutionTimes(configuration);

        // Assert
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
        //Arrange
        var configuration = new UnsupportedConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01))
        };

        //Act
        Action act = () => _factory.CreateScheduleType(configuration, 12);

        //Assert
        act.Should().Throw<ArgumentException>("Unsupported configuration type.");
    }
    [Fact]
    public void CalculateHours_ShouldReturnCorrectHours_WithHourlyInterval()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 02)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Interval = 2,
            IntervalType = IntervalType.Hourly
        };

        // Act
        var scheduleType = _factory.CreateScheduleType(configuration, 12).GetNextExecutionTimes(configuration);

        // Assert
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
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 02)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(9, 10, 0)),
            Interval = 5,
            IntervalType = IntervalType.Minutely
        };

        // Act
        var scheduleType = _factory.CreateScheduleType(configuration, 12).GetNextExecutionTimes(configuration);

        // Assert
        scheduleType.Should().HaveCount(3);
        scheduleType[0].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 9, 0, 0));
        scheduleType[1].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 9, 5, 0));
        scheduleType[2].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 9, 10, 0));
    }

    [Fact]
    public void CalculateHours_ShouldReturnCorrectSeconds_WithSecondlyInterval()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 02)),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(9, 0, 10)),
            Interval = 5,
            IntervalType = IntervalType.Secondly
        };

        // Act
        var scheduleType = _factory.CreateScheduleType(configuration, 12).GetNextExecutionTimes(configuration);

        // Assert
        scheduleType.Should().HaveCount(3);
        scheduleType[0].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 9, 0, 0));
        scheduleType[1].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 9, 0, 5));
        scheduleType[2].ExecutionTime.Should().Be(new DateTime(2024, 01, 01, 9, 0, 10));
    }

    [Fact]
    public void CreateSchedule_ShouldReturnCorrectDescription_WhenOnceSchedulerConfigurationIsValid()
    {
        //Arrange
        var configuration = new OnceSchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 01, 01),
            ConfigurationDateTime = new DateTime(2024, 02, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 02, 01))
        };

        //Act
        var scheduleType = _factory.CreateScheduleType(configuration, 12).GetNextExecutionTimes(configuration);

        //Assert

        scheduleType[0].Description.Should().Be($"Occurs once. Schedule will be used on {configuration.ConfigurationDateTime:dd/MM/yyyy} at {configuration.ConfigurationDateTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");

    }

    [Fact]
    public void CreateSchedule_ShouldReturnCorrectDescription_WhenDailyFrequencyConfigurationHourTimeRangeIsTheSame()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 03, 20, 00, 00)),
            HourTimeRange = new HourTimeRange(new TimeSpan(20, 00, 00), new TimeSpan(20, 00, 00)),
            Interval = 1,
            IntervalType = IntervalType.Hourly
        };

        // Act
        var scheduleType = _factory.CreateScheduleType(configuration, 12).GetNextExecutionTimes(configuration);

        // Assert
        scheduleType.Should().HaveCount(3); // Asegurarse de que hay 3 elementos en la lista

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
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
            WeekInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0))
        };
        var executionTime = new DateTime(2024, 03, 04, 9, 0, 0);

        // Act
        var description = _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        description.Should().Be("Occurs every 1 week(s) on Monday, Wednesday. Schedule will be used on 04/03/2024 at 09:00 starting on 01/03/2024.");
    }

    [Fact]
    public void GenerateExecutions_ShouldReturnCorrectExecutionTimes_ForOnceSchedulerConfiguration()
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            ConfigurationDateTime = new DateTime(2024, 03, 05, 9, 0, 0),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 06))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
        executionTimes.Should().HaveCount(1);
        executionTimes[0].Should().Be(new DateTime(2024, 03, 05, 9, 0, 0));
    }


    [Fact]
    public void GenerateExecutions_ShouldReturnCorrectExecutionTimes_ForDailyFrequencyConfiguration()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Interval = 2,
            IntervalType = 0,
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 03))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
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
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
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
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
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
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = false,
            ConfigurationDateTime = new DateTime(2024, 03, 05, 9, 0, 0)
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
        executionTimes.Should().BeEmpty();
    }
    [Fact]
    public void DailyFrequencyConfiguration_ShouldReturnCorrectExecutionTimes_WithDailyInterval()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            Interval = 2,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 03))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
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
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
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
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
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
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            Interval = 1,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 03))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
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
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
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
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 03, 01),
            IsEnabled = true,
            Interval = 3,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(21, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 02))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
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
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
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
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            CurrentDate = new DateTime(2023, 01, 01),
            IsEnabled = true,
            ConfigurationDateTime = new DateTime(2024, 01, 01),
            Limits = new LimitsTimeInterval(new DateTime(2023, 01, 01), new DateTime(2024, 01, 02))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
        executionTimes.Should().ContainSingle()
            .Which.Should().Be(new DateTime(2024, 01, 01));
    }

    [Fact]
    public void Configuration_WithLimitDateInPast_ShouldHandleCorrectly()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2023, 01, 01), new DateTime(2023, 12, 31))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
        executionTimes.Should().BeEmpty();
    }

    [Fact]
    public void Configuration_WithLimitDateInFuture_ShouldHandleCorrectly()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Interval = 2, 
            IntervalType = IntervalType.Hourly,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0)), 
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 05))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
        executionTimes.Should().HaveCount(4);
    }

    [Fact]
    public void Configuration_WithHourRangeCrossingMidnight_ShouldHandleCorrectly()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Interval = 2,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(22, 0, 0), new TimeSpan(2, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 02, 0, 00, 00))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
        executionTimes.Should().HaveCount(2);
    }

    [Fact]
    public void Configuration_Null_ShouldThrowException()
    {
        // Arrange
        SchedulerConfiguration configuration = null;

        // Act
        Action act = () => _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }


    [Fact]
    public void Configuration_InvalidHourlyInterval_ShouldThrowException()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Interval = -1,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 02))
        };

        // Act
        Action act = () => _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Invalid interval");
    }


    [Fact]
    public void Configuration_WithLeapYear_ShouldHandleCorrectly()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 02, 28),
            IsEnabled = true,
            Interval = 24,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 02, 28), new DateTime(2024, 03, 01))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
        executionTimes.Should().Contain(new DateTime(2024, 02, 29));
    }

    [Fact]
    public void Configuration_WithLeapYearInFebruary_ShouldHandleCorrectly()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 02, 28),
            IsEnabled = true,
            Interval = 24,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 02, 28), new DateTime(2024, 02, 29))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
        executionTimes.Should().Contain(new DateTime(2024, 02, 29));
    }

    [Fact]
    public void Configuration_WithNonLeapYearInFebruary_ShouldHandleCorrectly()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2023, 02, 28),
            IsEnabled = true,
            Interval = 24,
            IntervalType = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59)),
            Limits = new LimitsTimeInterval(new DateTime(2023, 02, 28), new DateTime(2023, 03, 01))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
        executionTimes.Should().Contain(new DateTime(2023, 02, 28));
        executionTimes.Should().Contain(new DateTime(2023, 03, 01));
    }
    [Fact]
    public void Configuration_WithComplexIntervals_ShouldHandleCorrectly()
    {
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
        executionTimes.Should().HaveCount(12);
    }

    [Fact]
    public void Configuration_WithComplexHourRanges_ShouldHandleCorrectly()
    {
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
        executionTimes.Should().HaveCount(12);
    }


    [Fact]
    public void Configuration_WithWeekendExclusion_ShouldHandleCorrectly()
    {
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
        executionTimes.Should().OnlyContain(dt => dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday);
    }

    [Fact]
    public void Configuration_WithWeekendOnly_ShouldHandleCorrectly()
    {
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration, 12);

        // Assert
        executionTimes.Should().OnlyContain(dt => dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday);
    }

    [Fact]
    public void ValidateConfiguration_WithDateTimeInPast_ShouldReturnException()
    {
        //Arrange
        var configuration = new OnceSchedulerConfiguration()
        {
            ConfigurationDateTime = new DateTime(2023, 01, 01),
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01))
        };

        //Act
        Action act = () => _timeGenerator.GenerateExecutions(configuration, 12);


        //Assert
        act.Should().Throw<ArgumentException>("Configuration date time cannot be in the past.");

    }
    [Fact]
    public void ValidateConfiguration_WithNullOnceConfiguration_ShouldReturnException()
    {
        //Arrange
        var invalidConfig = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Interval = 1,
            IntervalType = IntervalType.Hourly
        };

        //Act
        Action act = () => new OnceDateCalculator().CalculateDates(invalidConfig, 12);


        //Assert
        act.Should().Throw<ArgumentException>("Invalid configuration type for OnceDateCalculator.");

    }

    [Fact]
    public void GenerateExecutions_ShouldThrowException_ForUnknownConfigurationType()
    {
        // Arrange
        var unknownConfiguration = new UnsupportedConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
        };

        // Act
        Action act = () => _timeGenerator.GenerateExecutions(unknownConfiguration, 12);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Unknown configuration type");
    }
    [Fact]
    public void MonthlyFrequency_WithAllFirstMondayInAYear_ShouldReturnCorrectly()
    {
        // Arrange
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
            HourTimeRange = new HourTimeRange(new TimeSpan(13,00,00), new TimeSpan(13, 00, 00)) 
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        // Assert
        executionTimes.Should().HaveCount(12);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 01,13,00,00));
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
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        // Assert
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
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        // Assert
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
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(config,2);

        // Assert
        executionTimes.Should().HaveCount(2);

        executionTimes[0].Should().Be(new DateTime(2024, 03, 02, 10, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 03, 03, 10, 00, 00));
    }
    [Fact]
    public void MonthlyFrequency_WithFirstDayEveryTwoMonths_ShouldReturnCorrectly()
    {
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        // Assert
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
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        // Assert
        executionTimes.Should().HaveCount(3);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 01, 8, 00, 00));
        executionTimes[1].Should().Be(new DateTime(2024, 05, 01, 8, 00, 00));
        executionTimes[2].Should().Be(new DateTime(2024, 09, 01, 8, 00, 00));
    }
    [Fact]
    public void MonthlyFrequency_WithThirdWeekdayInAYear_ShouldReturnCorrectly()
    {
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        // Assert
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
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        // Assert
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
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        // Assert
        executionTimes.Should().HaveCount(12);

        executionTimes[0].Should().Be(new DateTime(2024, 01, 20, 10, 00, 00)); // Tercer sábado de enero
        executionTimes[1].Should().Be(new DateTime(2024, 01, 21, 10, 00, 00)); // Tercer domingo de enero
        executionTimes[2].Should().Be(new DateTime(2024, 02, 17, 10, 00, 00)); // Tercer sábado de febrero
        executionTimes[3].Should().Be(new DateTime(2024, 02, 18, 10, 00, 00)); // Tercer domingo de febrero
        executionTimes[4].Should().Be(new DateTime(2024, 03, 16, 10, 00, 00)); // Tercer sábado de marzo
        executionTimes[5].Should().Be(new DateTime(2024, 03, 17, 10, 00, 00)); // Tercer domingo de marzo
        executionTimes[6].Should().Be(new DateTime(2024, 04, 20, 10, 00, 00)); // Tercer sábado de abril
        executionTimes[7].Should().Be(new DateTime(2024, 04, 21, 10, 00, 00)); // Tercer domingo de abril
        executionTimes[8].Should().Be(new DateTime(2024, 05, 18, 10, 00, 00)); // Tercer sábado de mayo
        executionTimes[9].Should().Be(new DateTime(2024, 05, 19, 10, 00, 00)); // Tercer domingo de mayo
        executionTimes[10].Should().Be(new DateTime(2024, 06, 15, 10, 00, 00)); // Tercer sábado de junio
        executionTimes[11].Should().Be(new DateTime(2024, 06, 16, 10, 00, 00)); // Tercer domingo de junio
    }

    [Fact]
    public void MonthlyFrequency_WithLastWeekdayInAYear_ShouldReturnCorrectly()
    {
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        // Assert
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
        // Arrange
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

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(config, 12);

        // Assert
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

}



public class UnsupportedConfiguration : SchedulerConfiguration;

