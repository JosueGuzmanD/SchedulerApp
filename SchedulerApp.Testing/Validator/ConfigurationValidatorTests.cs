using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Common.Validator;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.ValueObjects;

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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Validate_ShouldHandleVariousConfigurations(bool isEnabled)
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            IsEnabled = isEnabled,
            CurrentDate = new DateTime(2024, 01, 01),
            ConfigurationDateTime = new DateTime(2024, 01, 02)
        };

        // Act
        Action act = () => _validator.Validate(configuration);

        // Assert
        if (isEnabled)
        {
            act.Should().NotThrow<ArgumentException>();
        }
        else
        {
            act.Should().Throw<ArgumentException>().WithMessage("Configuration must be enabled.");
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Validate_ShouldHandleDifferentSchedulerConfigurations(bool isEnabled)
    {
        // Arrange
        var onceConfiguration = new OnceSchedulerConfiguration
        {
            IsEnabled = isEnabled,
            CurrentDate = new DateTime(2024, 01, 01),
            ConfigurationDateTime = new DateTime(2024, 01, 02)
        };

        var dailyConfiguration = new DailyFrequencyConfiguration
        {
            IsEnabled = isEnabled,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        var weeklyConfiguration = new WeeklyFrequencyConfiguration
        {
            IsEnabled = isEnabled,
            CurrentDate = new DateTime(2024, 01, 01),
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
            WeekInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        if (isEnabled)
        {
            _validator.Validate(onceConfiguration);
            _validator.Validate(dailyConfiguration);
            _validator.Validate(weeklyConfiguration);
        }
        else
        {
            Action actOnce = () => _validator.Validate(onceConfiguration);
            Action actDaily = () => _validator.Validate(dailyConfiguration);
            Action actWeekly = () => _validator.Validate(weeklyConfiguration);

            actOnce.Should().Throw<ArgumentException>().WithMessage("Configuration must be enabled.");
            actDaily.Should().Throw<ArgumentException>().WithMessage("Configuration must be enabled.");
            actWeekly.Should().Throw<ArgumentException>().WithMessage("Configuration must be enabled.");
        }
    }
}

