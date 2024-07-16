using FluentAssertions;
using SchedulerApplication.Common.Validator;
using SchedulerApplication.Models;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Models.ValueObjects;

namespace SchedulerApp.Testing.Validator;

public class ConfigurationValidatorTests
{
    private readonly ConfigurationValidator _validator;

    public ConfigurationValidatorTests()
    {
        _validator = new ConfigurationValidator();
    }

    [Fact]
    public void Validate_ShouldThrowException_WhenConfigurationIsDisabled()
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            IsEnabled = false,
            CurrentDate = new DateTime(2024, 01, 01),
            ConfigurationDateTime = new DateTime(2024, 01, 02)
        };

        // Act
        Action act = () => _validator.Validate(configuration);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Configuration must be enabled.");
    }

    [Fact]
    public void Validate_ShouldNotThrowException_WhenConfigurationIsEnabled()
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            ConfigurationDateTime = new DateTime(2024, 01, 02)
        };

        // Act
        Action act = () => _validator.Validate(configuration);

        // Assert
        act.Should().NotThrow<ArgumentException>();
    }

    [Fact]
    public void Validate_ShouldThrowException_ForDisabledDailyFrequencyConfiguration()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            IsEnabled = false,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            HourlyInterval = 1,
        };

        // Act
        Action act = () => _validator.Validate(configuration);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Configuration must be enabled.");
    }

    [Fact]
    public void Validate_ShouldNotThrowException_ForEnabledDailyFrequencyConfiguration()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            HourlyInterval = 1,
        };

        // Act
        Action act = () => _validator.Validate(configuration);

        // Assert
        act.Should().NotThrow<ArgumentException>();
    }

    [Fact]
    public void Validate_ShouldThrowException_ForDisabledWeeklyFrequencyConfiguration()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            IsEnabled = false,
            CurrentDate = new DateTime(2024, 01, 01),
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
            WeekInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            HourlyInterval = 1,
        };

        // Act
        Action act = () => _validator.Validate(configuration);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Configuration must be enabled.");
    }

    [Fact]
    public void Validate_ShouldNotThrowException_ForEnabledWeeklyFrequencyConfiguration()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
            WeekInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            HourlyInterval = 1,
        };

        // Act
        Action act = () => _validator.Validate(configuration);

        // Assert
        act.Should().NotThrow<ArgumentException>();
    }
}

