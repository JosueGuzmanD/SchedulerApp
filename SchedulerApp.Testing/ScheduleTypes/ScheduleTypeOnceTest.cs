using FluentAssertions;
using SchedulerApplication.Common.Validator;
using SchedulerApplication.Models;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Description;
using SchedulerApplication.Services.ExecutionTime;
using SchedulerApplication.Services.Interfaces;
using SchedulerApplication.Services.ScheduleTypes;
using SchedulerApplication.ValueObjects;

namespace SchedulerApp.Testing.ScheduleTypes;
/*
public class ScheduleTypeOnceTests
{
    private readonly IDescriptionService _descriptionService;
    private readonly IOnceExecutionService _onceExecutionService;
    private readonly IConfigurationValidator _validator;
    private readonly ScheduleTypeOnce _scheduleTypeOnce;

    public ScheduleTypeOnceTests()
    {
        _descriptionService = new DescriptionService();
        _onceExecutionService = new OnceExecutionService(new ConfigurationValidator());
        _validator = new ConfigurationValidator();
        _scheduleTypeOnce = new ScheduleTypeOnce(_descriptionService, _onceExecutionService, _validator);
    }

    [Fact]
    public void CreateScheduleOutput_ShouldReturnCorrectOutput_ForValidConfiguration()
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            ConfigurationDateTime = new DateTime(2024, 01, 02, 09, 0, 0),
        };

        // Act
        var result = _scheduleTypeOnce.GetNextExecutionTimes(configuration);

        // Assert
        result.Should().HaveCount(1);
        result[0].ExecutionTime.Should().Be(new DateTime(2024, 01, 02, 09, 0, 0));
        result[0].Description.Should().Be("Occurs once. Schedule will be used on 02/01/2024 at 09:00 starting on 01/01/2024.");
    }

    [Fact]
    public void CreateScheduleOutput_ShouldThrowException_WhenConfigurationIsDisabled()
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = false,
            ConfigurationDateTime = new DateTime(2024, 01, 02, 09, 0, 0),
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31))
        };

        // Act
        Action act = () => _scheduleTypeOnce.GetNextExecutionTimes(configuration);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Configuration must be enabled.");
    }

    [Fact]
    public void CreateScheduleOutput_ShouldThrowException_WhenConfigurationDateTimeIsInThePast()
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 03),
            IsEnabled = true,
            ConfigurationDateTime = new DateTime(2024, 01, 02, 09, 0, 0),
        };

        // Act
        Action act = () => _scheduleTypeOnce.GetNextExecutionTimes(configuration);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Configuration date time cannot be in the past.");
    }

    [Theory]
    [InlineData("2024-01-01", "2024-01-01 09:00:00", "2024-01-01 09:00:00", true)]
    [InlineData("2024-01-01", "2024-01-02 10:00:00", "2024-01-02 10:00:00", true)]
    [InlineData("2024-01-01", "2024-01-01 08:59:59", "2024-01-01 08:59:59", false)]
    public void CreateScheduleOutput_ShouldHandleVariousConfigurations(string currentDate, string configurationDateTime, string expectedExecutionTime, bool isEnabled)
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            CurrentDate = DateTime.Parse(currentDate),
            IsEnabled = isEnabled,
            ConfigurationDateTime = DateTime.Parse(configurationDateTime),
            Limits = new LimitsTimeInterval(DateTime.Parse(currentDate), new DateTime(2024, 12, 31))
        };

        if (!isEnabled || DateTime.Parse(configurationDateTime) < DateTime.Parse(currentDate))
        {
            // Act
            Action act = () => _scheduleTypeOnce.GetNextExecutionTimes(configuration);

            // Assert
            if (!isEnabled)
            {
                act.Should().Throw<ArgumentException>().WithMessage("Configuration must be enabled.");
            }
            else
            {
                act.Should().Throw<ArgumentException>().WithMessage("Configuration date time cannot be in the past.");
            }
        }
        else
        {
            // Act
            var result = _scheduleTypeOnce.GetNextExecutionTimes(configuration);

            // Assert
            result.Should().HaveCount(1);
            result[0].ExecutionTime.Should().Be(DateTime.Parse(expectedExecutionTime));
        }
    }
}

*/