using SchedulerApp.Domain.Entities;

namespace SchedulerApp.Domain.Interfaces
{
    public interface IScheduleType
    {
        Task<ScheduleOutput> getNextExecutionTimeAsync(SchedulerConfiguration configuration);
    }
}
