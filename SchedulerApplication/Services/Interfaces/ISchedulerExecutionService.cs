using SchedulerApplication.Models.SchedulerConfigurations;

namespace SchedulerApplication.Services.Interfaces;

public interface ISchedulerExecutionService
{
}

public interface IOnceExecutionService : ISchedulerExecutionService
{
    DateTime CalculateNextExecutionTime(OnceSchedulerConfiguration configuration);
}

public interface IRecurringExecutionService : ISchedulerExecutionService
{
    List<DateTime> CalculateNextExecutionTimes(RecurringSchedulerConfiguration configuration);
}
