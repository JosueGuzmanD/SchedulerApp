using SchedulerApplication.Models.FrequencyConfigurations;

namespace SchedulerApplication.Services.Interfaces;

public interface IWeeklyExecutionCalculatorService
{
    List<DateTime> CalculateWeeklyExecutions(WeeklyFrequencyConfiguration config, int maxExecutions);
}


