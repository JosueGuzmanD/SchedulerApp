using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.WeekCalculator;

public class WeeklyExecutionCalculatorService : IWeeklyExecutionCalculatorService
{
    public IEnumerable<DateTime> CalculateWeeklyExecutions(WeeklyFrequencyConfiguration config)
    {
        if (config == null)
            throw new ArgumentNullException();

        var results = new List<DateTime>();
        var currentDate = config.CurrentDate;
        var weekInterval = config.WeekInterval;
        var daysOfWeek = new HashSet<DayOfWeek>(config.DaysOfWeek);
        var limitInitDatetime = config.Limits.LimitStartDateTime;
        var endDate = config.Limits.LimitEndDateTime ?? DateTime.MaxValue;

        if (!daysOfWeek.Any())
        {
            return results;
        }

        while (currentDate <= endDate && results.Count < 12)
        {
            if (daysOfWeek.Contains(currentDate.DayOfWeek))
            {
                var executionTime = currentDate.Date;
                if (executionTime >= limitInitDatetime && executionTime <= endDate)
                {
                    results.Add(executionTime);
                }
            }

            currentDate = currentDate.AddDays(1);

            if (currentDate.DayOfWeek == DayOfWeek.Monday && results.Count > 0)
            {
                currentDate = currentDate.AddDays(7 * (weekInterval - 1));
            }
        }

        return results;
    }
}


