using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using SchedulerApplication.Common.Validator;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.ExecutionTime;

namespace SchedulerApp.Testing.ExecutionTime;

public class ScheduleTypeOnceTests
{

    [Theory]
    [InlineData("2024-06-20T20:12:32")]
    [InlineData("2004-05-21T01:13:32")]
    [InlineData("2014-03-22T23:12:32")]
    public void CalculateNextExecutionTime_ShouldReturnCurrentDate_WhenDateTimeIsValid(string currentDate)
    {
        var dateTime = DateTime.Parse(currentDate);
        //Arrange
        var configuration = new OnceSchedulerConfiguration()
        {
            IsEnabled = true,
            CurrentDate = dateTime
        };
        var validator = new ConfigurationValidator();
        var service = new OnceExecutionService(validator);

        //Act
        var result = service.CalculateNextExecutionTime(configuration);

        //Assert
        result.Should().Be(configuration.CurrentDate);
    }

    [Fact]
    public void CalculateNextExecutionTime_ShouldThrowException_WhenIsEnableEqualsFalse()
    {
        //Arrange
        var configuration = new OnceSchedulerConfiguration() { IsEnabled = false };
        var validator = new ConfigurationValidator();
        var service = new OnceExecutionService(validator);

        //Act
        Action act = () => service.CalculateNextExecutionTime(configuration);

        //Assert
        act.Should().Throw<ArgumentException>("Configuration must be enabled.");
    }


}

