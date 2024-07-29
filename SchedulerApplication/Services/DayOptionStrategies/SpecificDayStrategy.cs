using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.FrequencyConfigurations;

namespace SchedulerApplication.Services.DayOptionStrategies;

public class SpecificDayStrategy : IDateCalculationStrategy
{
    public List<DateTime> CalculateDates(MonthlySchedulerConfiguration config, int maxExecutions)
    {
        if (config is not SpecificDayMonthlySchedulerConfiguration specificDayConfig)
        {
            throw new ArgumentException("Invalid configuration type for SpecificDayStrategy.");
        }

        var list = new List<DateTime>();
        var actualDateTime = specificDayConfig.CurrentDate;
        var dayOfMonth = specificDayConfig.SpecificDay;

        while (list.Count < maxExecutions)
        {
            var nextDate = new DateTime(actualDateTime.Year, actualDateTime.Month, dayOfMonth);
            if (nextDate >= specificDayConfig.Limits.LimitStartDateTime && nextDate <= specificDayConfig.Limits.LimitEndDateTime)
            {
                list.Add(nextDate);
            }
            actualDateTime = actualDateTime.AddMonths(specificDayConfig.MonthFrequency);
        }

        return list;
    }
}