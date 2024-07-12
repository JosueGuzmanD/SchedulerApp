using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Models;

namespace SchedulerApplication.Interfaces;

    public interface IExecutionTimeGenerator
    {
        IEnumerable<DateTime> GenerateExecutions(SchedulerConfiguration config);
    }

    public interface IOnceExecutionCalculator
    {
        IEnumerable<DateTime> CalculateOnceExecutions(OnceSchedulerConfiguration config);
    }

    public interface IDailyExecutionCalculator
    {
        IEnumerable<DateTime> CalculateDailyExecutions(DailyFrequencyConfiguration config);
    }

    public interface IWeeklyExecutionCalculator
    {
        IEnumerable<DateTime> CalculateWeeklyExecutions(WeeklyFrequencyConfiguration config);
    }


