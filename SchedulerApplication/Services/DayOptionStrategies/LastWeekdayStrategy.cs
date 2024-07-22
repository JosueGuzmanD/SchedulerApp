using SchedulerApplication.Common.Enums;
using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models;

namespace SchedulerApplication.Services.DayOptionStrategies;

public class LastWeekdayStrategy : IDateCalculationStrategy
{
    public List<DateTime> CalculateDates(SchedulerConfiguration config)
    {
        if (config is not MonthlySchedulerConfiguration monthlyConfig)
        {
            throw new ArgumentException("Invalid configuration type for LastWeekdayStrategy.");
        }

        return AddLastWeekdays(monthlyConfig.WeekOption, monthlyConfig.CurrentDate, monthlyConfig.MonthFrequency, monthlyConfig.MaxExecutions);
    }

    private static List<DateTime> AddLastWeekdays(WeekOptions weekOption, DateTime actualDateTime, int monthFrequency, int maxExecutions)
    {
        var list = new List<DateTime>();
        while (list.Count < maxExecutions)
        {
            actualDateTime = new DateTime(actualDateTime.Year, actualDateTime.Month, DateTime.DaysInMonth(actualDateTime.Year, actualDateTime.Month));

            while (!IsValidDay(weekOption, actualDateTime))
            {
                actualDateTime = actualDateTime.AddDays(-1);
            }

            list.Add(actualDateTime);
            actualDateTime = actualDateTime.AddMonths(monthFrequency);
        }
        return list;
    }

    private static bool IsValidDay(WeekOptions weekOption, DateTime date)
    {
        return weekOption switch
        {
            WeekOptions.Weekday => date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday,
            WeekOptions.WeekendDay => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday,
            WeekOptions.AnyDay => true,
            _ => date.DayOfWeek == (DayOfWeek)weekOption
        };
    }
}
