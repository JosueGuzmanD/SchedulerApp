﻿using FluentAssertions;
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
            new HourCalculatorService());
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
        var scheduleType = _factory.CreateScheduleType(configuration);
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
            HourlyInterval = 2,
            HourTimeRange = new HourTimeRange(new TimeSpan(21, 00, 00), new TimeSpan(23, 00, 00)),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 07, 15), new DateTime(2024, 07, 17))
        };

        // Act
        var scheduleType = _factory.CreateScheduleType(configuration);
        var executionTimes = scheduleType.GetNextExecutionTimes(configuration);

        // Assert
        scheduleType.Should().BeOfType<ScheduleTypeRecurring>();
        executionTimes.Should().HaveCount(6);

        var expectedTimes = new List<DateTime>
        {
            new (2024, 07, 15, 21, 00, 00),
            new (2024, 07, 15, 23, 00, 00),
            new (2024, 07, 16, 21, 00, 00),
            new (2024, 07, 16, 23, 00, 00),
            new (2024, 07, 17, 21, 00, 00),
            new (2024, 07, 17, 23, 00, 00)
        };

        for (var i = 0; i < expectedTimes.Count; i++)
        {
            executionTimes[i].ExecutionTime.Should().Be(expectedTimes[i]);
            executionTimes[i].Description.Should().Be($@"Occurs every day from {configuration.HourTimeRange.StartHour:hh\:mm} to {configuration.HourTimeRange.EndHour:hh\:mm}. Schedule will be used on {expectedTimes[i]:dd/MM/yyyy} at {expectedTimes[i]:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
        }
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
        Action act = () => _factory.CreateScheduleType(configuration);

        //Assert
        act.Should().Throw<ArgumentException>("Unsupported configuration type.");
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
        var scheduleType = _factory.CreateScheduleType(configuration).GetNextExecutionTimes(configuration);

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
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 03)),
            HourTimeRange = new HourTimeRange(new TimeSpan(20, 00, 00), new TimeSpan(20, 00, 00))
        };

        // Act
        var scheduleType = _factory.CreateScheduleType(configuration).GetNextExecutionTimes(configuration);
        var expectedTimes = new List<DateTime>
        {
            new (2024, 03, 01, 20, 00, 00),
            new (2024, 03, 02, 20, 00, 00),
            new (2024, 03, 03, 20, 00, 00)
        };

        // Assert
        for (var i = 0; i < expectedTimes.Count; i++)
        {
            scheduleType[i].ExecutionTime.Should().Be(expectedTimes[i]);
            scheduleType[i].Description.Should()
                .Be(
                    $"Occurs every day from {configuration.HourTimeRange.StartHour:hh\\:mm} to {configuration.HourTimeRange.EndHour:hh\\:mm}. Schedule will be used on {expectedTimes[i]:dd/MM/yyyy} at {expectedTimes[i]:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
        }
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
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

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
            HourlyInterval = 2,
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 03))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

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
            new (2024, 03, 03, 9, 0, 0),
            new (2024, 03, 03, 11, 0, 0)
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
            HourlyInterval = 2,
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 15))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

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
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

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
            HourlyInterval = 2,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 03))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

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
            new (2024, 03, 03, 9, 0, 0),
            new (2024, 03, 03, 11, 0, 0)
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
            HourlyInterval = 2,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 15))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

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
            DaysOfWeek = [DayOfWeek.Monday],
            WeekInterval = 2,
            HourlyInterval = 2,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 31))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

        // Assert
        var expectedTimes = new List<DateTime>
        {
            new (2024, 03, 18, 9, 0, 0),
            new (2024, 03, 18, 11, 0, 0),
            new (2024, 03, 18, 13, 0, 0),
            new (2024, 03, 18, 15, 0, 0),
            new (2024, 03, 18, 17, 0, 0),
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
            HourlyInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 03))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

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
            HourlyInterval = 2,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 15))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

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
            HourlyInterval = 3,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(21, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 02))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

        // Assert
        var expectedTimes = new List<DateTime>
        {
            new (2024, 03, 01, 9, 0, 0),
            new (2024, 03, 01, 12, 0, 0),
            new (2024, 03, 01, 15, 0, 0),
            new (2024, 03, 01, 18, 0, 0),
            new (2024, 03, 01, 21, 0, 0),
            new (2024, 03, 02, 9, 0, 0),
            new (2024, 03, 02, 12, 0, 0),
            new (2024, 03, 02, 15, 0, 0),
            new (2024, 03, 02, 18, 0, 0),
            new (2024, 03, 02, 21, 0, 0)
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
            HourlyInterval = 3,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(21, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 03, 01), new DateTime(2024, 03, 31))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

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
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

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
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

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
            HourlyInterval = 24,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 05))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

        // Assert
        executionTimes.Should().HaveCount(5);
    }

    [Fact]
    public void Configuration_WithHourRangeCrossingMidnight_ShouldHandleCorrectly()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            HourlyInterval = 2,
            HourTimeRange = new HourTimeRange(new TimeSpan(22, 0, 0), new TimeSpan(2, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 02))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

        // Assert
        executionTimes.Should().HaveCount(12);
    }

    [Fact]
    public void Configuration_Null_ShouldThrowException()
    {
        // Arrange
        SchedulerConfiguration configuration = null;

        // Act
        Action act = () => _timeGenerator.GenerateExecutions(configuration);

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
            HourlyInterval = -1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 02))
        };

        // Act
        Action act = () => _timeGenerator.GenerateExecutions(configuration);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Invalid hourly interval");
    }


    [Fact]
    public void Configuration_WithLeapYear_ShouldHandleCorrectly()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 02, 28),
            IsEnabled = true,
            HourlyInterval = 24,
            HourTimeRange = new HourTimeRange(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 02, 28), new DateTime(2024, 03, 01))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

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
            HourlyInterval = 24,
            HourTimeRange = new HourTimeRange(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 02, 28), new DateTime(2024, 02, 29))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

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
            HourlyInterval = 24,
            HourTimeRange = new HourTimeRange(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59)),
            Limits = new LimitsTimeInterval(new DateTime(2023, 02, 28), new DateTime(2023, 03, 01))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

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
            HourlyInterval = 3,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(15, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 31))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

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
            HourlyInterval = 2,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(21, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 15))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

        // Assert
        executionTimes.Should().HaveCount(12);
    }

    [Fact]
    public void Configuration_WithZeroHourlyInterval_ShouldHandleCorrectly()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            HourlyInterval = 0,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 02))
        };

        // Act
        Action act = () => _timeGenerator.GenerateExecutions(configuration);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Invalid hourly interval");
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
            HourlyInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 31))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

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
            HourlyInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 01, 31))
        };

        // Act
        var executionTimes = _timeGenerator.GenerateExecutions(configuration);

        // Assert
        executionTimes.Should().OnlyContain(dt => dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday);
    }

}

    public class UnsupportedConfiguration: SchedulerConfiguration;

