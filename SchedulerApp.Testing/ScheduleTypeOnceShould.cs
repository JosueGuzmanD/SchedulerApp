using SchedulerApp.Application.Services;
using SchedulerApp.Domain.Common.Enums;
using SchedulerApp.Domain.Entities;

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
                Date = DateTime.Now,
                Frequency = 0,
                DaysInterval = 0,
                LimitEndDateTime = DateTime.Now,
                LimitStartDateTime = DateTime.Now,
            };

            var output = new ScheduleTypeSelector().GetScheduleType;

        }
    }

