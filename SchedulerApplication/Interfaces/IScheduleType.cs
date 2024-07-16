using SchedulerApplication.Models;

namespace SchedulerApplication.Interfaces;

// Allows different schedule types (once, recurring),  enabling the factory to return a common interface.
public interface IScheduleType
{
    List<ScheduleOutput> GetNextExecutionTimes(SchedulerConfiguration configuration);
}
