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

        while (results.Count < 12 && currentDate <= weeklyConfig.Limits.LimitEndDateTime)
        {
            // Check if currentDate is one of the specified days of the week and within the limits
            if (weeklyConfig.DaysOfWeek.Contains(currentDate.DayOfWeek) && currentDate >= weeklyConfig.Limits.LimitStartDateTime)
            {
                results.Add(currentDate);
            }

            // Move to the next day
            currentDate = currentDate.AddDays(1);

            // Check if we need to jump weeks
            if (currentDate.DayOfWeek == DayOfWeek.Monday && weeklyConfig.WeekInterval > 1)
            {
                currentDate = currentDate.AddDays(7 * (weeklyConfig.WeekInterval - 1));
            }
        }

        return results;
    }
}

