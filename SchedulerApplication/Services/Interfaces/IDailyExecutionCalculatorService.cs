using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.ValueObjects;

namespace SchedulerApplication.Services.Interfaces;

public interface IDailyExecutionCalculatorService
{
    List<DateTime> CalculateDailyExecutions(DailyFrequencyConfiguration config, int maxExecutions);

    IEnumerable<DateTime> GenerateHourlyExecutionsForDay(HourTimeRange hourTimeRange, DateTime day,
        LimitsTimeInterval limits, int interval);
}

