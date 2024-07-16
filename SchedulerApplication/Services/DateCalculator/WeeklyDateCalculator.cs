using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models;

namespace SchedulerApplication.Services.DateCalculator;

public class WeeklyDateCalculator : IDateCalculator
{
    public List<DateTime> CalculateDates(SchedulerConfiguration config)
    {
        var weeklyConfig = (WeeklyFrequencyConfiguration)config;
        var results = new List<DateTime>();
        var currentDate = weeklyConfig.CurrentDate;
        var needInterval = true;

        while (results.Count < 12 && currentDate <= weeklyConfig.Limits.LimitEndDateTime)
        {
            if (weeklyConfig.DaysOfWeek.Contains(currentDate.DayOfWeek))
            {
                results.Add(currentDate);
                needInterval = true;
            }

            if (needInterval)
            {
                currentDate=currentDate.AddDays(7*weeklyConfig.WeekInterval);
                needInterval=false;
            }
            currentDate = currentDate.AddDays(1);
        }

        return results;
    }
}
