using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.FrequencyConfigurations;

namespace SchedulerApplication.Services.HourCalculatorServices;

public class WeeklyExecutionCalculator : IWeeklyExecutionCalculator
{
    public IEnumerable<DateTime> CalculateWeeklyExecutions(WeeklyFrequencyConfiguration config)
    {
        var results = new List<DateTime>();
        var currentDate = config.CurrentDate;
        var startTime = config.HourTimeRange.StartHour;
        var endTime = config.HourTimeRange.EndHour;
        var interval = config.HourlyInterval;
        var startLimit = config.Limits.LimitStartDateTime;
        var endLimit = config.Limits.LimitEndDateTime ?? DateTime.MaxValue;
        var weekInterval = config.WeekInterval;
        var daysOfWeek = new HashSet<DayOfWeek>(config.DaysOfWeek);

        while (currentDate <= endLimit && results.Count < 12)
        {
            if (daysOfWeek.Contains(currentDate.DayOfWeek))
            {
                var currentHour = currentDate.Date.Add(startTime);

                while (currentHour.TimeOfDay <= endTime && results.Count < 12 && currentHour <= endLimit)
                {
                    if (currentHour >= startLimit && currentHour <= endLimit)
                    {
                        results.Add(currentHour);
                    }
                    currentHour = currentHour.AddHours(interval);
                }
            }

            currentDate = currentDate.AddDays(1);

            if (currentDate.DayOfWeek == DayOfWeek.Monday && currentDate != config.CurrentDate)
            {
                currentDate = currentDate.AddDays(7 * (weekInterval - 1));
            }
        }

        return results;
    }
}