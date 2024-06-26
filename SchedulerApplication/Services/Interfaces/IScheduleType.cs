using SchedulerApp.Domain.Entities;

namespace SchedulerApp.Domain.Interfaces;

    public interface IScheduleType
    {
        ScheduleOutput getNextExecutionTime(SchedulerConfiguration configuration);
    }

