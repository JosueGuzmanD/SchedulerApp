using SchedulerApp.Domain.Entities;

namespace SchedulerApp.Domain.Interfaces
{
    public interface IScheduleService
    {
        Task<ScheduleOutput> getNextExecutionTimeAsync();
    }
}
