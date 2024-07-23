using SchedulerApplication.Common.Enums;
using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models;

namespace SchedulerApplication.Services.DayOptionStrategies;

public class LastWeekdayStrategy : IDateCalculationStrategy
{
    public List<DateTime> CalculateDates(MonthlySchedulerConfiguration config, int maxExecutions)
    {
        var list = new List<DateTime>();
        var actualDateTime = config.CurrentDate;

        while (list.Count < maxExecutions)
        {
            var startDate = new DateTime(actualDateTime.Year, actualDateTime.Month, DateTime.DaysInMonth(actualDateTime.Year, actualDateTime.Month));

            if (config.WeekOption == WeekOptions.WeekendDay)
            {
                var weekendDates = AddLastWeekendDates(config.WeekOption, actualDateTime, config.MonthFrequency, maxExecutions);
                list.AddRange(weekendDates);
            }
            else
            {
                while (!IsValidDay(config.WeekOption, startDate))
                {
                    startDate = startDate.AddDays(-1);
                }
                list.Add(startDate);
            }

            actualDateTime = actualDateTime.AddMonths(config.MonthFrequency);
        }

        return list;
    }

    private bool IsValidDay(WeekOptions weekOption, DateTime date)
    {
        return weekOption switch
        {
            WeekOptions.Weekday => date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday,
            WeekOptions.WeekendDay => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday,
            WeekOptions.AnyDay => true,
            _ => date.DayOfWeek == (DayOfWeek)weekOption
        };
    }

    private List<DateTime> AddLastWeekendDates(WeekOptions weekOption, DateTime actualDateTime, int monthFrequency, int maxExecutions)
    {
        var list = new List<DateTime>();
        while (list.Count < maxExecutions)
        {
            var startDate = new DateTime(actualDateTime.Year, actualDateTime.Month, DateTime.DaysInMonth(actualDateTime.Year, actualDateTime.Month));

            while (startDate.Month == actualDateTime.Month)
            {
                if (startDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    list.Add(startDate);
                    list.Add(startDate.AddDays(1)); // Add Sunday
                    break;
                }
                else if (startDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    list.Add(startDate.AddDays(-1)); // Add Saturday
                    list.Add(startDate); // Add Sunday
                    break;
                }
                else
                {
                    startDate = startDate.AddDays(-1); // Move to previous day
                }
            }

            actualDateTime = actualDateTime.AddMonths(monthFrequency);
        }
        return list;
    }
}
