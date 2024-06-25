using SchedulerApp.Application.Services;
using SchedulerApp.Domain.Common.Enums;
using SchedulerApp.Domain.Entities;
using FluentAssertions;

namespace SchedulerApp.Testing;

    public class ScheduleTypeOnceShould
    {
        [Fact]
        public void GetNextExecutionTime_ShouldReturnSingleExecution()
        {
            var fixedDate = new DateTime(2025, 12, 25, 10, 0, 0);

        //Arrange
        var config = new SchedulerConfiguration()
            {
                Type = SchedulerType.Once,
                IsEnabled = true,
                StartDate = fixedDate,
                Frequency = 0,
                DaysInterval = 0,
                LimitEndDateTime = fixedDate.Date,
                LimitStartDateTime = fixedDate.Date,
            };
            var type = new ScheduleTypeOnce();

            var result = type.getNextExecutionTime(config);

            result.ExecutionTime.Should().ContainSingle().Which.Should().Be(fixedDate.Date);

            result.Description.Should().Contain($"Occurs Once. Schedule will be used on {fixedDate:dd/MM/yyyy} at {fixedDate.Hour} starting on {fixedDate:dd/MM/yyyy}");
    }

    [Fact]
    public void GetNextExecutionTime_ShouldThrowExceptionWhenConfigurationNotEnabled()
    {
        var config = new SchedulerConfiguration()
        {
            Type = SchedulerType.Once,
            IsEnabled = false,
            StartDate = DateTime.Now.Date,
            Frequency = 0,
            DaysInterval = 0,
            LimitEndDateTime = DateTime.Now.Date,
            LimitStartDateTime = DateTime.Now.Date,
        };

        var type = new ScheduleTypeOnce();

        Action act = () => type.getNextExecutionTime(config);

        act.Should().Throw<InvalidOperationException>().WithMessage("You must enable a configuration type.");
    }

    [Fact]
    public void GetNextExecutionTime_ShouldHandlePastStartDate()
    {
        var pastDate = new DateTime(2025, 06, 10, 10, 0, 0);
        var config = new SchedulerConfiguration()
        {
            Type = SchedulerType.Once,
            IsEnabled = true,
            StartDate = pastDate,
            DaysInterval = 0,
            LimitEndDateTime = pastDate.Date,
            LimitStartDateTime = pastDate.Date
        };


    }


}

