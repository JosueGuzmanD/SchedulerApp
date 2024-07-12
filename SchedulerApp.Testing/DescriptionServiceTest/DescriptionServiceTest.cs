﻿using FluentAssertions;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Models.ValueObjects;
using SchedulerApplication.Services.Description;

namespace SchedulerApp.Testing.DescriptionServiceTest;

public class DescriptionServiceTests
{
    private readonly DescriptionService _descriptionService;

    public DescriptionServiceTests()
    {
        _descriptionService = new DescriptionService();
    }

    [Fact]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForOnceScheduler()
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            ConfigurationDateTime = new DateTime(2024, 01, 01, 9, 0, 0)
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        var result = _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        result.Should().Be($"Occurs once. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
    }

    [Fact]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForDailyFrequency_Once_09_00()
    {
        // Arrange
        var onceAt = new TimeSpan(9, 0, 0);
        var configuration = new DailyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(onceAt)
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        var result = _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        result.Should().Be($"Occurs once at 09:00. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
    }

    [Fact]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForDailyFrequency_Once_10_00()
    {
        // Arrange
        var onceAt = new TimeSpan(10, 0, 0);
        var configuration = new DailyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(onceAt)
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        var result = _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        result.Should().Be($"Occurs once at 10:00. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
    }

    [Fact]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForDailyFrequency_Recurrent()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0))
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        var result = _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        result.Should().Be($"Occurs every day from 09:00 to 17:00. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
    }

    [Theory]
    [InlineData(1, "Occurs every 1 week(s) on Monday, Wednesday")]
    [InlineData(2, "Occurs every 2 week(s) on Monday, Wednesday")]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForWeeklyFrequency(int weekInterval, string expectedDescription)
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            WeekInterval = weekInterval,
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0))
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        var result = _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        result.Should().Be($"{expectedDescription}. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
    }

    [Fact]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForWeeklyFrequency_WithMultipleDays()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            WeekInterval = 2,
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0))
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        var result = _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        result.Should().Be($"Occurs every 2 week(s) on Monday, Wednesday. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
    }

    [Fact]
    public void GenerateDescription_DailyConfig_NullHourTimeRange_ShouldThrowArgumentNullException()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = null
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        Action act = () => _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GenerateDescription_WeeklyConfig_NullHourTimeRange_ShouldThrowArgumentNullException()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            WeekInterval = 1,
            DaysOfWeek = [DayOfWeek.Monday],
            HourTimeRange = null
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        Action act = () => _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GenerateDescription_WeeklyConfig_NullDaysOfWeek_ShouldThrowArgumentNullException()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            WeekInterval = 1,
            DaysOfWeek = null,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0))
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        Action act = () => _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GenerateDescription_WeeklyConfig_EmptyDaysOfWeek_ShouldThrowArgumentNullException()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            WeekInterval = 1,
            DaysOfWeek = [],
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0))
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        Action act = () => _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}


