using FluentAssertions;
using SchedulerApplication.Common.Validator;
using SchedulerApplication.Models;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Description;
using SchedulerApplication.Services.ExecutionTime;
using SchedulerApplication.Services.ScheduleTypes;
using SchedulerApplication.ValueObjects;

namespace SchedulerApp.Testing.ScheduleTypes;

public class ScheduleTypeOnceTest
{
    [Fact]
    public void CreateScheduleOutput_ShouldReturnExampleScheduleOutput_WhenConfigurationIsValid()
    {
        // Arrange
        var validator = new ConfigurationValidator();
        var configuration = new OnceSchedulerConfiguration
        {
            CurrentDate = new DateTime(2020,01,04),
            IsEnabled = true,
            ConfigurationDateTime = new DateTime(2020,01,08,14,00,00),
            TimeInterval = new TimeInterval(new DateTime(2020,01,01))
        };


        var onceExecutionService = new OnceExecutionService(validator);
        var descriptionService = new DescriptionService();

        var expectedList = new List<ScheduleOutput>
        {
            new()
            {
                Description = $"Occurs Once. Schedule will be used on 08/01/2020 at 14:00 starting on 01/01/2020.",
                ExecutionTime = new DateTime(2020,01,08,14,00,00)
            }
        };

        var scheduleTypeOnce = new ScheduleTypeOnce(descriptionService, onceExecutionService, validator);

        // Act
        var result = scheduleTypeOnce.GetNextExecutionTimes(configuration);

        // Assert
        result.Should().BeEquivalentTo(expectedList);
    }

    [Fact]
    public void CreateScheduleOutput_ShouldThrowException_IfIsEnabledIsFalse()
    {
        //Arrange
        var validator = new ConfigurationValidator();
        var executionService = new OnceExecutionService(validator);
        var descriptionService = new DescriptionService();
        var configuration = new OnceSchedulerConfiguration()
        {
            CurrentDate = new DateTime(2024, 02, 12, 21, 12, 32),
            IsEnabled = false
        };

        //Act   
        Action act = () =>
            new ScheduleTypeOnce(descriptionService, executionService, validator).GetNextExecutionTimes(configuration);

        //Assert
        act.Should().Throw<ArgumentException>("Configuration must be enabled.");
    }

    [Theory]
    [InlineData(2020, 01, 01, 2020, 01, 08, 14, 00, 00)]
    [InlineData(2021, 03, 01, 2021, 03, 15, 10, 30, 00)]
    [InlineData(2022, 06, 15, 2022, 06, 25, 12, 00, 00)]
    public void CreateScheduleOutput_ShouldReturnCorrectDescriptionAndExecutionTime(int startYear, int startMonth, int startDay, int execYear, int execMonth, int execDay, int execHour, int execMinute, int execSecond)
    {
        // Arrange
        var validator = new ConfigurationValidator();
        var configuration = new OnceSchedulerConfiguration
        {
            CurrentDate = new DateTime(startYear, startMonth, startDay),
            IsEnabled = true,
            ConfigurationDateTime = new DateTime(execYear, execMonth, execDay, execHour, execMinute, execSecond),
            TimeInterval = new TimeInterval(new DateTime(startYear, startMonth, startDay))
        };

        var onceExecutionService = new OnceExecutionService(validator);
        var descriptionService = new DescriptionService();

        var expectedList = new List<ScheduleOutput>
        {
            new()
            {
                Description = $"Occurs Once. Schedule will be used on {execDay:D2}/{execMonth:D2}/{execYear} at {execHour:D2}:{execMinute:D2} starting on {startDay:D2}/{startMonth:D2}/{startYear}.",
                ExecutionTime = new DateTime(execYear, execMonth, execDay, execHour, execMinute, execSecond)
            }
        };

        var scheduleTypeOnce = new ScheduleTypeOnce(descriptionService, onceExecutionService, validator);

        // Act
        var result = scheduleTypeOnce.GetNextExecutionTimes(configuration);

        // Assert
        result.Should().BeEquivalentTo(expectedList);
    }

    [Fact]
    public void CreateScheduleOutput_ShouldThrowException_WhenConfigurationDateTimeIsInPast()
    {
        // Arrange
        var validator = new ConfigurationValidator();
        var configuration = new OnceSchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 02, 12),
            IsEnabled = true,
            ConfigurationDateTime = new DateTime(2023, 02, 10),
            TimeInterval = new TimeInterval(new DateTime(2024, 01, 01))
        };

        var onceExecutionService = new OnceExecutionService(validator);
        var descriptionService = new DescriptionService();

        var scheduleTypeOnce = new ScheduleTypeOnce(descriptionService, onceExecutionService, validator);

        // Act
        Action act = () => scheduleTypeOnce.GetNextExecutionTimes(configuration);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Configuration date time cannot be in the past.");
    }
}

