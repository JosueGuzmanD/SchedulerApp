using SchedulerApp.Domain.Common.Enums;
using SchedulerApp.Domain.Entities;
using FluentAssertions;
using SchedulerApplication.Services.Implementations;
using SchedulerApplication.ValueObjects;

namespace SchedulerApp.Testing;

    public class ScheduleTypeOnceShould
    {
    [Theory]
    [InlineData(2025, 12, 25, 10, 0, 0)]
    [InlineData(2024, 06, 10, 10, 0, 0)]
    [InlineData(2025, 12, 12, 10, 0, 0)]
    public void GetNextExecutionTime_WhenConfigurationEnabled_ShouldReturnSingleExecution(
        int year, int month, int day, int hour, int minute, int second)
    {
        // Arrange
        var fixedDate = new DateTime(year, month, day, hour, minute, second);
        var config = new SchedulerConfiguration()
        {
            Type = SchedulerType.Once,
            IsEnabled = true,
            Frequency = Frequency.Daily,
            CurrentDate = fixedDate,
            TimeInterval = new TimeInterval(fixedDate.Date, fixedDate.Date)
        };
        var type = new ScheduleTypeOnce();

        // Act
        var result = type.getNextExecutionTime(config);

        // Assert
        result.ExecutionTime.Should().Be(fixedDate);
        result.Description.Should().Contain($"Occurs {config.Type}. Schedule will be used on {fixedDate:dd/MM/yyyy} at {fixedDate:HH:mm} starting on {fixedDate:dd/MM/yyyy}.");
    }

    [Fact]
    public void GetNextExecutionTime_WhenConfigurationDisabled_ShouldThrowException()
    {
        // Arrange
        var config = new SchedulerConfiguration()
        {
            Type = SchedulerType.Once,
            IsEnabled = false,
            Frequency = Frequency.Daily,
            CurrentDate = DateTime.Now.Date,
            TimeInterval = new TimeInterval(DateTime.Now.Date, DateTime.Now.Date)
        };

        var type = new ScheduleTypeOnce();

        // Act
        Action act = () => type.getNextExecutionTime(config);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Configuration must be enabled.");
    }

    [Theory]
    [InlineData(2024, 06, 10, 10, 0, 0)]
    [InlineData(2025, 12, 12, 10, 0, 0)]
    public void GetNextExecutionTime_WhenStartDateIsSet_ShouldReturnSingleExecution(
        int year, int month, int day, int hour, int minute, int second)
    {
        // Arrange
        var startDate = new DateTime(year, month, day, hour, minute, second);
        var config = new SchedulerConfiguration()
        {
            Type = SchedulerType.Once,
            IsEnabled = true,
            Frequency = Frequency.Daily,
            CurrentDate = startDate,
            TimeInterval = new TimeInterval(startDate.Date, startDate.Date)
        };
        var type = new ScheduleTypeOnce();

        // Act
        var result = type.getNextExecutionTime(config);

        // Assert
        result.ExecutionTime.Should().Be(startDate);
        result.Description.Should().Be($"Occurs {config.Type}. Schedule will be used on {startDate:dd/MM/yyyy} at {startDate:HH:mm} starting on {startDate:dd/MM/yyyy}.");
    }
}

