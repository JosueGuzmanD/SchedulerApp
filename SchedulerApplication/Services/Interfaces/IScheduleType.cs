using SchedulerApp.Domain.Entities;

namespace SchedulerApplication.Services.Interfaces;

public interface IScheduleType
{
    List<ScheduleOutput> GetNextExecutionTimes(SchedulerConfiguration configuration);
}

