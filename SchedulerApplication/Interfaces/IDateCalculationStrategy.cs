using SchedulerApplication.Models;
using SchedulerApplication.Models.FrequencyConfigurations;

namespace SchedulerApplication.Interfaces;

public interface IDateCalculationStrategy
{
    List<DateTime> CalculateDates(MonthlySchedulerConfiguration config, int maxExecutions);
}

