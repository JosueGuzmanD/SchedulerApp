using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.ValueObjects;

namespace SchedulerApplication.Interfaces;

public interface IDailyExecutionCalculatorService
{
    List<DateTime> CalculateDailyExecutions(DailyFrequencyConfiguration config, int maxExecutions);

    IEnumerable<DateTime> GenerateHourlyExecutionsForDay(HourTimeRange hourTimeRange, DateTime day,
        LimitsTimeInterval limits, int interval);
}

