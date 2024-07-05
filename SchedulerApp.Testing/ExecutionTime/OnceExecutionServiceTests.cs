using FluentAssertions;
using SchedulerApplication.Common.Validator;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.ExecutionTime;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApp.Testing.ExecutionTime;

public class OnceExecutionServiceTests
{
    private readonly IConfigurationValidator _validator;
    private readonly IOnceExecutionService _onceExecutionService;

    public OnceExecutionServiceTests()
    {
        _validator = new ConfigurationValidator();
        _onceExecutionService = new OnceExecutionService(_validator);
    }

    [Fact]
    public void CalculateNextExecutionTime_ShouldThrowException_WhenConfigurationIsDisabled()
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            IsEnabled = false,
            CurrentDate = DateTime.Now,
            ConfigurationDateTime = DateTime.Now.AddHours(1)
        };

        // Act
        Action act = () => _onceExecutionService.CalculateNextExecutionTime(configuration);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Configuration must be enabled.");
    }

    [Fact]
    public void CalculateNextExecutionTime_ShouldThrowException_WhenConfigurationDateTimeIsInThePast()
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = DateTime.Now,
            ConfigurationDateTime = DateTime.Now.AddHours(-1)
        };

        // Act
        Action act = () => _onceExecutionService.CalculateNextExecutionTime(configuration);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Configuration date time cannot be in the past.");
    }

    [Fact]
    public void CalculateNextExecutionTime_ShouldReturnCorrectExecutionTime_WhenConfigurationIsValid()
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = DateTime.Now,
            ConfigurationDateTime = DateTime.Now.AddHours(1)
        };

        // Act
        var result = _onceExecutionService.CalculateNextExecutionTime(configuration);

        // Assert
        result.Should().Be(configuration.ConfigurationDateTime);
    }

    [Theory]
    [InlineData("2022-01-01", "2024-01-01")]
    [InlineData("2024-01-03", "2024-01-21")]
    public void CalculateNextExecutionTime_ShouldThrowException_WhenConfigurationDateTimeIsBeforeCurrentDate(string configDateTime, string currentDate)
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = DateTime.Parse(currentDate),
            ConfigurationDateTime = DateTime.Parse(configDateTime)
        };

        // Act
        Action act = () => _onceExecutionService.CalculateNextExecutionTime(configuration);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Configuration date time cannot be in the past.");
    }

    [Theory]
    [InlineData("2024-01-01T11:00:00", "2024-01-01T10:00:00")]
    [InlineData("2024-01-01T16:00:00", "2024-01-01T15:59:59")]
    public void CalculateNextExecutionTime_ShouldReturnCorrectExecutionTime_WhenConfigurationDateTimeIsAfterCurrentDate(string configDateTime, string currentDate)
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = DateTime.Parse(currentDate),
            ConfigurationDateTime = DateTime.Parse(configDateTime)
        };

        // Act
        var result = _onceExecutionService.CalculateNextExecutionTime(configuration);

        // Assert
        result.Should().Be(DateTime.Parse(configDateTime));
    }
}