using SchedulerApplication.Models;

namespace SchedulerApplication.Interfaces;

public interface IScheduleType
{
    List<ScheduleOutput> GetNextExecutionTimes(SchedulerConfiguration configuration);
}

