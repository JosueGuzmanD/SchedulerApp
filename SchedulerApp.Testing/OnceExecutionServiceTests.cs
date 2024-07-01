using FluentAssertions;
using Moq;
using SchedulerApp.Domain.Entities;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;
using SchedulerApplication.Services.ScheduleTypes;

namespace SchedulerApp.Testing;

public class ScheduleTypeOnceTests
{
    [Fact]
    public void GetNextExecutionTimes_ShouldThrowException_WhenConfigurationIsInvalid()
    {
        // Arrange
        var descriptionServiceMock = new Mock<IDescriptionService>();
        var onceExecutionServiceMock = new Mock<IOnceExecutionService>();
        var validatorMock = new Mock<IConfigurationValidator>();
        validatorMock.Setup(v => v.Validate(It.IsAny<SchedulerConfiguration>()))
            .Throws(new ArgumentException("Configuration must be enabled."));

        var configuration = new OnceSchedulerConfiguration 
        {
            IsEnabled = false,
            CurrentDate = new DateTime(2024, 06, 25)
        };

        var scheduleType = new ScheduleTypeOnce(descriptionServiceMock.Object, onceExecutionServiceMock.Object, validatorMock.Object);

        // Act
        Action act = () => scheduleType.GetNextExecutionTimes(configuration);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Configuration must be enabled.");
    }

    [Fact]
    public void GetNextExecutionTimes_ShouldReturnSingleExecutionTime()
    {
        // Arrange
        var descriptionServiceMock = new Mock<IDescriptionService>();
        var onceExecutionServiceMock = new Mock<IOnceExecutionService>();
        var validatorMock = new Mock<IConfigurationValidator>();

        var configuration = new OnceSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 06, 25)
        };

        onceExecutionServiceMock.Setup(s => s.CalculateNextExecutionTime(configuration)).Returns(configuration.CurrentDate);
        descriptionServiceMock.Setup(s => s.GenerateDescription(configuration, configuration.CurrentDate))
                              .Returns("Occurs Once. Schedule will be used on 25/06/2024 at 00:00 starting on 25/06/2024.");

        var scheduleType = new ScheduleTypeOnce(descriptionServiceMock.Object, onceExecutionServiceMock.Object, validatorMock.Object);

        // Act
        var result = scheduleType.GetNextExecutionTimes(configuration);

        // Assert
        result.Should().HaveCount(1);
        result[0].ExecutionTime.Should().Be(configuration.CurrentDate);
        result[0].Description.Should().Be("Occurs Once. Schedule will be used on 25/06/2024 at 00:00 starting on 25/06/2024.");
    }

    [Theory]
    [InlineData(2024, 6, 25)]
    [InlineData(2025, 12, 25)]
    public void GetNextExecutionTimes_ShouldReturnCorrectDate(int year, int month, int day)
    {
        // Arrange
        var date = new DateTime(year, month, day);
        var descriptionServiceMock = new Mock<IDescriptionService>();
        var onceExecutionServiceMock = new Mock<IOnceExecutionService>();
        var validatorMock = new Mock<IConfigurationValidator>();

        var configuration = new OnceSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = date
        };

        onceExecutionServiceMock.Setup(s => s.CalculateNextExecutionTime(configuration)).Returns(date);
        descriptionServiceMock.Setup(s => s.GenerateDescription(configuration, date))
                              .Returns($"Occurs Once. Schedule will be used on {date:dd/MM/yyyy} at {date:HH:mm} starting on {date:dd/MM/yyyy}.");

        var scheduleType = new ScheduleTypeOnce(descriptionServiceMock.Object, onceExecutionServiceMock.Object, validatorMock.Object);

        // Act
        var result = scheduleType.GetNextExecutionTimes(configuration);

        // Assert
        result.Should().HaveCount(1);
        result[0].ExecutionTime.Should().Be(date);
        result[0].Description.Should().Be($"Occurs Once. Schedule will be used on {date:dd/MM/yyyy} at {date:HH:mm} starting on {date:dd/MM/yyyy}.");
    }
}

