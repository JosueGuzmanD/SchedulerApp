using SchedulerApplication.Common.Enums;
using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.FrequencyConfigurations;

namespace SchedulerApplication.Services.DayOptionStrategies;

public class OrdinalWeekdayStrategy : IDateCalculationStrategy
{
    private readonly int _ordinal;

    public OrdinalWeekdayStrategy(int ordinal)
    {
        _ordinal = ordinal;
    }

    public List<DateTime> CalculateDates(MonthlySchedulerConfiguration config, int maxExecutions)
    {
        var list = new List<DateTime>();
        var actualDateTime = config.CurrentDate;

        while (list.Count < maxExecutions)
        {
            var startDate = new DateTime(actualDateTime.Year, actualDateTime.Month, 1);
            var count = 0;

            if (config.WeekOption == WeekOptions.WeekendDay)
            {
                list.AddRange(AddWeekendDates(startDate, config.MonthFrequency, maxExecutions, _ordinal));
            }
            else
            {
                while (count < _ordinal)
                {
                    if (IsValidDay(config.WeekOption, startDate))
                    {
                        count++;
                        if (count == _ordinal && startDate.Day <= _ordinal * 7)
                        {
                            list.Add(startDate);
                        }
                    }
                    startDate = startDate.AddDays(1);
                }
            }

            actualDateTime = actualDateTime.AddMonths(config.MonthFrequency);
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

    private static List<DateTime> AddWeekendDates(DateTime actualDateTime, int monthFrequency, int maxExecutions, int ordinal)
    {
        var list = new List<DateTime>();
        while (list.Count < maxExecutions)
        {
            var count = 0;
            var startDate = new DateTime(actualDateTime.Year, actualDateTime.Month, 1);

            while (startDate.Month == actualDateTime.Month)
            {
                if (startDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    count++;
                    if (count == ordinal)
                    {
                        list.Add(startDate);
                        list.Add(startDate.AddDays(1));
                        break;
                    }
                    startDate = startDate.AddDays(7);
                }
                else if (startDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    count++;
                    if (count == ordinal)
                    {
                        list.Add(startDate.AddDays(-1));
                        list.Add(startDate);
                        break;
                    }
                    startDate = startDate.AddDays(6);
                }
                else
                {
                    startDate = startDate.AddDays(1);
                }
            }

            actualDateTime = actualDateTime.AddMonths(monthFrequency);
        }
        return list;
    }
}

