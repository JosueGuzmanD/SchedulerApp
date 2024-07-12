using SchedulerApplication.Models.FrequencyConfigurations;

namespace SchedulerApplication.Interfaces;

public interface IWeeklyExecutionCalculatorService
{
    List<DateTime> CalculateWeeklyExecutions(WeeklyFrequencyConfiguration config, int maxExecutions);
}


