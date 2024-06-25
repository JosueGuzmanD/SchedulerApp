using SchedulerApp.Application.Services;
using SchedulerApp.Domain.Common.Enums;
using SchedulerApp.Domain.Entities;
using Xunit;
using FluentAssertions;

namespace SchedulerApp.Testing;

    public class ScheduleTypeOnceShould
    {
        [Fact]
        public void GetNextExecutionTime_ShouldReturnSingleExecution()
        {
            //Arrange
            var config = new SchedulerConfiguration()
            {
                Type = SchedulerType.Once,
                IsEnabled = true,
                Date = DateTime.Now.Date,
                Frequency = 0,
                DaysInterval = 0,
                LimitEndDateTime = DateTime.Now.Date,
                LimitStartDateTime = DateTime.Now.Date,
            };
            var type = new ScheduleTypeOnce();

            var result = type.getNextExecutionTime(config);

            result.ExecutionTime.Should().ContainSingle().Which.Should().Be(DateTime.Now.Date);

            result.Description.Should().Contain($"Occurs Once. Schedule will be used on {DateTime.Now.Date} at {DateTime.Now.Hour} starting on {DateTime.Now.Date}");


        }
    }

