using FluentAssertions;
using SchedulerApp.Application.Services;
using SchedulerApp.Domain.Common.Enums;
using SchedulerApp.Domain.Entities;
using DateTime = System.DateTime;

namespace SchedulerApp.Testing;

    public class ScheduleTypeRecurringShould
    {
        [Fact]
        public void GetNextExecutionTime_ConfigurationDisabled_ShouldThrowException()
        {
            //Arrange
            var startingDate = new DateTime(2024, 06, 21, 10, 10, 20);


            var configuration = new SchedulerConfiguration()
            {
                IsEnabled = false,
                Type = SchedulerType.Recurring,
                StartDate = startingDate,
                LimitStartDateTime = startingDate.Date,
                LimitEndDateTime = startingDate.Date.AddDays(4)
            };

            var type = new ScheduleTypeRecurring();

            //Act
            var act = ()=> type.getNextExecutionTime(configuration);

            //Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("You must enable a configuration type.");
    }

        [Fact]
        public void GetNextExecutionTime_ConfigurationEnabled_ShouldReturnExecution()
        {
            //Arrange
            var startingDate = new DateTime(2024, 06, 25, 10, 0, 10);
            var configuration = new SchedulerConfiguration()
            {
                IsEnabled = true,
                Type = SchedulerType.Recurring,
                StartDate = startingDate,
                DaysInterval = 1,
                LimitStartDateTime = startingDate.Date,
                LimitEndDateTime = startingDate.Date.AddDays(2)
            };
            var type = new ScheduleTypeSelector().GetScheduleType(configuration.Type);

            //Act
            var act = type.getNextExecutionTime(configuration);

            //Assert
            var expectedDates = new[]
            {
                startingDate.Date,
                startingDate.Date.AddDays(1),
                startingDate.Date.AddDays(2)
             };

            act.ExecutionTime.Should().BeEquivalentTo(expectedDates);
            act.Description.Should().Contain($"Occurs every day. Schedule will be used on {startingDate.Date:dd/MM/yy} at {startingDate.Hour} starting on {startingDate.Date:dd/MM/yy}");

        }

        [Fact]
        public void GetNextExecutionTime_PastStartDate_ShouldReturnExecution()
        {
            //Arrange
            var startingDate = new DateTime(2024,04,20,10, 0, 0);
            var configuration = new SchedulerConfiguration()
            {
                IsEnabled = true,
                Type = SchedulerType.Recurring,
                StartDate = startingDate,
                DaysInterval = 1,
                LimitStartDateTime = startingDate.Date,
                LimitEndDateTime = startingDate.Date.AddDays(2)
            };
            var type = new ScheduleTypeSelector().GetScheduleType(configuration.Type);

            //Act
            var act = type.getNextExecutionTime(configuration);

            //Assert

            var expectedDates = new[]
            {
                startingDate.Date,
                startingDate.Date.AddDays(1),
                startingDate.Date.AddDays(2)
            };

            act.ExecutionTime.Should().BeEquivalentTo(expectedDates);
            act.Description.Should().Contain($"Occurs every day. Schedule will be used on {startingDate.Date:dd/MM/yy} at {startingDate.Hour} starting on {startingDate.Date:dd/MM/yy}");
    }

        [Fact]
        public void GetNextExecutionTime_MaxExecutions_ShouldReturnMaxExecutionDescription()
        {
            //Assert
            var startingDate = new DateTime(2024, 06, 26, 10, 0, 0);
            var configuration = new SchedulerConfiguration()
            {
                IsEnabled = true,
                StartDate = startingDate,
                DaysInterval = 1,
                LimitStartDateTime = startingDate.Date,
                LimitEndDateTime = startingDate.Date.AddDays(87),
                Type = SchedulerType.Recurring
            };

            //Act 
            var type = new ScheduleTypeSelector().GetScheduleType(configuration.Type).getNextExecutionTime(configuration);

            //Assert
            var expectedDates = new []
            {
                startingDate,
                startingDate.AddDays(1),
                startingDate.AddDays(2)
            };

            type.ExecutionTime.Should().BeEquivalentTo(expectedDates);
            type.Description.Should().Contain("Execution times are capped at 3 entries.");


    }


}

