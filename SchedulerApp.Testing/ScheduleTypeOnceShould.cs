using SchedulerApp.Application.Services;
using SchedulerApp.Domain.Common.Enums;
using SchedulerApp.Domain.Entities;
using FluentAssertions;

namespace SchedulerApp.Testing;

    public class ScheduleTypeOnceShould
    {
        [Fact]
        public void GetNextExecutionTime_ConfigurationEnabled_ShouldReturnSingleExecution()
        {
            var fixedDate = new DateTime(2025, 12, 25, 10, 0, 0);

        //Arrange
        var config = new SchedulerConfiguration()
            {
                Type = SchedulerType.Once,
                IsEnabled = true,
                Frequency = SchedulerFrequency.Daily,
                StartDate = fixedDate,
                LimitEndDateTime = fixedDate.Date,
                LimitStartDateTime = fixedDate.Date,
            };
            var type = new ScheduleTypeOnce();
            //Act

            var result = type.getNextExecutionTime(config);

            //Assert

            result.ExecutionTime.Should().ContainSingle().Which.Should().Be(fixedDate.Date);

            result.Description.Should().Contain($"Occurs Once. Schedule will be used on {fixedDate:dd/MM/yyyy} at {fixedDate.Hour} starting on {fixedDate:dd/MM/yyyy}");
    }

    [Fact]
    public void GetNextExecutionTime_ConfigurationDisabled_ShouldThrowException()
    {
        //Arrange
        var config = new SchedulerConfiguration()
        {
            Type = SchedulerType.Once,
            IsEnabled = false,
            Frequency = SchedulerFrequency.Daily,
            StartDate = DateTime.Now.Date,
            LimitEndDateTime = DateTime.Now.Date,
            LimitStartDateTime = DateTime.Now.Date,
        };

        var type = new ScheduleTypeOnce();

        //Act
        Action act = () => type.getNextExecutionTime(config);

        //Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("You must enable a configuration type.");
    }

    [Fact]
    public void GetNextExecutionTime_StartDateInPast_ShouldReturnSingleExecution()
    {
        //Arrange
        var pastDate = new DateTime(2024, 06, 10, 10, 0, 0);
        var config = new SchedulerConfiguration()
        {
            Type = SchedulerType.Once,
            IsEnabled = true,
            Frequency = SchedulerFrequency.Daily,
            StartDate = pastDate,
            LimitEndDateTime = pastDate.Date,
            LimitStartDateTime = pastDate.Date
        };

        var type= new ScheduleTypeOnce();

        //Act
        var result= type.getNextExecutionTime(config);

        //Assert
        result.ExecutionTime.Should().ContainSingle()
            .Which.Should().Be(pastDate.Date);

        result.Description.Should()
            .Be($"Occurs Once. Schedule will be used on {pastDate:dd/MM/yyyy} at {pastDate.Hour} starting on {pastDate:dd/MM/yyyy}");
    }

    [Fact]
    public void GetNextExecutionTime_StartDateInFuture_ShouldReturnSingleExecution()
    {
        //Arrange
        DateTime futureDate = new DateTime(2025, 12, 12, 10, 0,0);

        var config = new SchedulerConfiguration()
        {
            IsEnabled = true,
            Type = SchedulerType.Once,
            Frequency = SchedulerFrequency.Daily,
            StartDate = futureDate,
            LimitStartDateTime = futureDate.AddDays(2).Date,
            LimitEndDateTime = futureDate.AddDays(2).Date
        };
        var type= new ScheduleTypeOnce();

        //Act
        var result=type.getNextExecutionTime(config);

        //Assert
        result.ExecutionTime.Should().ContainSingle().Which.Should().Be(futureDate.Date);

        result.Description.Should()
            .Be($"Occurs Once. Schedule will be used on {futureDate:dd/MM/yyyy} at {futureDate.Hour} starting on {config.LimitStartDateTime:dd/MM/yyyy}");
    }
}

