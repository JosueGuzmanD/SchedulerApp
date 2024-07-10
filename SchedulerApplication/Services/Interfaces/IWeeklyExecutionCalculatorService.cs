using SchedulerApplication.Models.FrequencyConfigurations;

namespace SchedulerApplication.Services.Interfaces;

public interface IWeeklyExecutionCalculatorService
{
    IEnumerable<DateTime> CalculateWeeklyExecutions(WeeklyFrequencyConfiguration config);
}


