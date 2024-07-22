using SchedulerApplication.Common.Enums;
using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models;

namespace SchedulerApplication.Services.DayOptionStrategies;

public class OrdinalWeekdayStrategy : IDateCalculationStrategy
{
    private readonly int _ordinal;

    public OrdinalWeekdayStrategy(int ordinal)
    {
        _ordinal = ordinal;
    }

    public List<DateTime> CalculateDates(SchedulerConfiguration config)
    {
        if (config is not MonthlySchedulerConfiguration monthlyConfig)
        {
            throw new ArgumentException("Invalid configuration type for OrdinalWeekdayStrategy.");
        }

        return AddOrdinalWeekdays(monthlyConfig.WeekOption, monthlyConfig.CurrentDate, monthlyConfig.MonthFrequency, monthlyConfig.MaxExecutions, _ordinal);
    }

    private static List<DateTime> AddOrdinalWeekdays(WeekOptions weekOption, DateTime actualDateTime, int monthFrequency, int maxExecutions, int ordinal)
    {
        var list = new List<DateTime>();
        while (list.Count < maxExecutions)
        {
            actualDateTime = new DateTime(actualDateTime.Year, actualDateTime.Month, 1);
            int count = 0;

            while (count < ordinal)
            {
                if (IsValidDay(weekOption, actualDateTime))
                {
                    count++;
                    if (count == ordinal && actualDateTime.Day <= ordinal * 7)
                    {
                        list.Add(actualDateTime);
                    }
                }
                actualDateTime = actualDateTime.AddDays(1);
            }

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

