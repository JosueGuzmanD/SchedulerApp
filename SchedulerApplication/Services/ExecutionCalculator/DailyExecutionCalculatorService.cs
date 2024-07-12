using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Services.Interfaces;
using SchedulerApplication.ValueObjects;

namespace SchedulerApplication.Services.ExecutionCalculator;

public class DailyExecutionCalculatorService : IDailyExecutionCalculatorService
{
    public List<DateTime> CalculateDailyExecutions(DailyFrequencyConfiguration config, int maxExecutions)
    {
        var executionTimes = new List<DateTime>();
        var currentDate = config.CurrentDate;
        var endDate = config.Limits.LimitEndDateTime ?? DateTime.MaxValue;

        while (currentDate <= endDate && executionTimes.Count < maxExecutions)
        {
            if (config.OccursOnce)
            {
                var executionTime = currentDate.Date + config.OnceAt;
                if (config.Limits.IsWithinInterval(executionTime))
                {
                    executionTimes.Add(executionTime);
                }
            }
            else
            {
                var hourlyExecutions = GenerateHourlyExecutionsForDay(config.HourTimeRange, currentDate, config.Limits, config.HourlyInterval);
                foreach (var executionTime in hourlyExecutions)
                {
                    if (executionTimes.Count >= maxExecutions || executionTime > endDate) break;
                    if (config.Limits.IsWithinInterval(executionTime))
                    {
                        executionTimes.Add(executionTime);
                    }
                }
            }

            currentDate = currentDate.AddDays(1);
        }

        return executionTimes;
    }

    public IEnumerable<DateTime> GenerateHourlyExecutionsForDay(HourTimeRange hourTimeRange, DateTime day, LimitsTimeInterval limits, int interval)
    {
        var results = new List<DateTime>();
        var currentHour = day.Date.Add(hourTimeRange.StartHour);
        var endHour = day.Date.Add(hourTimeRange.EndHour);

        while (currentHour <= endHour && results.Count < 12)
        {
            if (currentHour >= limits.LimitStartDateTime && currentHour <= limits.LimitEndDateTime)
            {
                results.Add(currentHour);
            }
            currentHour = currentHour.AddHours(interval);
        }

        return results;
    }
}
