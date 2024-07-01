using SchedulerApplication.Models;

namespace SchedulerApplication.Services.Interfaces;

public interface IScheduleType
{
    List<ScheduleOutput> GetNextExecutionTimes(SchedulerConfiguration configuration);
}

