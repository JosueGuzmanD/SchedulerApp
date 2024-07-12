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
        var act = () => _validator.Validate(configuration);

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
        var act = () => _validator.Validate(configuration);

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
        var act = () => _validator.Validate(configuration);

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
    [MemberData(nameof(GetSchedulerConfigurations))]
    public void Validate_ShouldHandleDifferentSchedulerConfigurations(SchedulerConfiguration configuration, bool shouldThrow)
    {
        // Act
        var act = () => _validator.Validate(configuration);

        // Assert
        if (shouldThrow)
        {
            act.Should().Throw<ArgumentException>().WithMessage("Configuration must be enabled.");
        }
        else
        {
            act.Should().NotThrow<ArgumentException>();
        }
    }

    public static IEnumerable<object[]> GetSchedulerConfigurations()
    {
        yield return new object[]
        {
            new OnceSchedulerConfiguration
            {
                IsEnabled = false,
                CurrentDate = new DateTime(2024, 01, 01),
                ConfigurationDateTime = new DateTime(2024, 01, 02)
            },
            true
        };

        yield return new object[]
        {
            new OnceSchedulerConfiguration
            {
                IsEnabled = true,
                CurrentDate = new DateTime(2024, 01, 01),
                ConfigurationDateTime = new DateTime(2024, 01, 02)
            },
            false
        };

        yield return new object[]
        {
            new DailyFrequencyConfiguration
            {
                IsEnabled = false,
                CurrentDate = new DateTime(2024, 01, 01),
                HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
                HourlyInterval = 1,
            },
            true
        };

        yield return new object[]
        {
            new DailyFrequencyConfiguration
            {
                IsEnabled = true,
                CurrentDate = new DateTime(2024, 01, 01),
                HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
                HourlyInterval = 1,
            },
            false
        };

        yield return new object[]
        {
            new WeeklyFrequencyConfiguration
            {
                IsEnabled = false,
                CurrentDate = new DateTime(2024, 01, 01),
                DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
                WeekInterval = 1,
                HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
                HourlyInterval = 1,
            },
            true
        };

        yield return new object[]
        {
            new WeeklyFrequencyConfiguration
            {
                IsEnabled = true,
                CurrentDate = new DateTime(2024, 01, 01),
                DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
                WeekInterval = 1,
                HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
                HourlyInterval = 1,
            },
            false
        };
    }
}

