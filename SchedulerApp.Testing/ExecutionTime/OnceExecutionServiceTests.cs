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
    [MemberData(nameof(ExecutionTimeTestDataGenerator.GetExecutionTimeTestData), MemberType = typeof(ExecutionTimeTestDataGenerator))]
    public void CalculateNextExecutionTime_ShouldReturnCorrectExecutionTime_WhenConfigurationDateTimeIsAfterCurrentDate(ExecutionTimeTestData testData)
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = testData.CurrentDate,
            ConfigurationDateTime = testData.ConfigurationDateTime
        };

        // Act
        var result = _onceExecutionService.CalculateNextExecutionTime(configuration);

        // Assert
        result.Should().Be(testData.ConfigurationDateTime);
    }
}