using FluentAssertions;
using SchedulerApp.Domain.Common.Enums;
using SchedulerApp.Domain.Entities;
using SchedulerApplication.Services.Implementations;
using SchedulerApplication.ValueObjects;
using DateTime = System.DateTime;

namespace SchedulerApp.Testing;

    public class ScheduleTypeRecurringShould
{
    [Theory]
    [InlineData("2024-06-25T10:00:10", 1, "2024-06-26T10:00:10", "Occurs every day. Schedule will be used on 26/06/2024 at 10:00 starting on 25/06/2024.")]
    [InlineData("2024-06-25T10:00:10", 2, "2024-06-27T10:00:10", "Occurs every 2 days. Schedule will be used on 27/06/2024 at 10:00 starting on 25/06/2024.")]
    [InlineData("2024-06-25T10:00:10", 3, "2024-06-28T10:00:10", "Occurs every 3 days. Schedule will be used on 28/06/2024 at 10:00 starting on 25/06/2024.")]
    public void GetNextExecutionTime_WhenConfigurationEnabled_ShouldReturnNextExecutionTime(string startDateString, int interval, string expectedDateString, string expectedDescription)
    {
        // Arrange
        var startDate = DateTime.Parse(startDateString);
        var expectedDate = DateTime.Parse(expectedDateString);
        var configuration = new SchedulerConfiguration
        {
            IsEnabled = true,
            Type = SchedulerType.Recurring,
            CurrentDate = startDate,
            DaysInterval = interval,
            TimeInterval = new TimeInterval(startDate, startDate.AddDays(3))
        };
        var type = new ScheduleTypeRecurring();

        // Act
        var result = type.getNextExecutionTime(configuration);

        // Assert
        result.ExecutionTime.Should().Be(expectedDate);
        result.Description.Should().Be(expectedDescription);
    }

    [Fact]
    public void GetNextExecutionTime_ConfigurationDisabled_ShouldThrowException()
    {
        // Arrange
        var startingDate = new DateTime(2024, 06, 21, 10, 10, 20);
        var configuration = new SchedulerConfiguration
        {
            IsEnabled = false,
            Type = SchedulerType.Recurring,
            CurrentDate = startingDate,
            TimeInterval = new TimeInterval(startingDate, startingDate.AddDays(4))
        };
        var type = new ScheduleTypeRecurring();

        // Act
        Action act = () => type.getNextExecutionTime(configuration);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Configuration must be enabled.");
    }

    [Fact]
    public void GetNextExecutionTime_MaxExecutions_ShouldReturnMaxExecutionDescription()
    {
        // Arrange
        var startingDate = new DateTime(2024, 06, 26, 10, 0, 0);
        var configuration = new SchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = startingDate,
            DaysInterval = 1,
            TimeInterval = new TimeInterval(startingDate, startingDate.AddDays(87)),
            Type = SchedulerType.Recurring
        };
        var type = new ScheduleTypeRecurring();

        // Act
        var result = type.getNextExecutionTime(configuration);

        // Assert
        var expectedDate = startingDate.AddDays(1);
        result.ExecutionTime.Should().Be(expectedDate);
        result.Description.Should().Be($"Occurs every day. Schedule will be used on {expectedDate:dd/MM/yyyy} at {expectedDate:HH:mm} starting on {startingDate:dd/MM/yyyy}.");
    }

    [Fact]
    public void GetNextExecutionTime_DifferentDaysInterval_ShouldReturnCorrectExecution()
    {
        // Arrange
        var startingDate = new DateTime(2024, 06, 25, 10, 0, 10);
        var configuration = new SchedulerConfiguration
        {
            IsEnabled = true,
            Type = SchedulerType.Recurring,
            CurrentDate = startingDate,
            DaysInterval = 2,
            TimeInterval = new TimeInterval(startingDate, startingDate.AddDays(4))
        };
        var type = new ScheduleTypeSelector().GetScheduleType(configuration.Type);

        // Act
        var result = type.getNextExecutionTime(configuration);

        // Assert
        var expectedDate = startingDate.AddDays(2);
        result.ExecutionTime.Should().Be(expectedDate);
        result.Description.Should().Be($"Occurs every 2 days. Schedule will be used on {expectedDate:dd/MM/yyyy} at {expectedDate:HH:mm} starting on {startingDate:dd/MM/yyyy}.");
    }
}