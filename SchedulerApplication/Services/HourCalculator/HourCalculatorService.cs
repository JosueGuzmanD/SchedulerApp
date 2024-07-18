using SchedulerApplication.Common.Enums;
using SchedulerApplication.Models.ValueObjects;

namespace SchedulerApplication.Services.HourCalculator;


public class HourCalculatorService 
{
   public List<DateTime> CalculateHours(List<DateTime> dates, HourTimeRange hourTimeRange, int interval, IntervalType intervalType, LimitsTimeInterval limits)
    {
        var results = new List<DateTime>();
        var endLimitTime = limits.LimitEndDateTime ?? DateTime.MaxValue;

        foreach (var date in dates)
        {
            var currentHour = date.Date.Add(hourTimeRange.StartHour);
            var endDateTime = date.Date.Add(hourTimeRange.EndHour);

            if (hourTimeRange.StartHour <= hourTimeRange.EndHour)
            {
                while (currentHour <= endDateTime && results.Count < 12 && currentHour <= endLimitTime)
                {
                    results.Add(currentHour);
                    currentHour = AddInterval(currentHour, interval, intervalType);
                }
            }
            else
            {
                // Handle the case where the time range crosses midnight
                while (currentHour.TimeOfDay < TimeSpan.FromHours(24) && results.Count < 12 && currentHour <= endLimitTime)
                {
                    results.Add(currentHour);
                    currentHour = AddInterval(currentHour, interval, intervalType);
                }

                currentHour = date.Date.AddDays(1).Add(hourTimeRange.StartHour);

                while (currentHour.TimeOfDay <= hourTimeRange.EndHour && results.Count < 12 && currentHour <= endLimitTime)
                {
                    results.Add(currentHour);
                    currentHour = AddInterval(currentHour, interval, intervalType);
                }
            }
        }

        return results;
    }

    private static DateTime AddInterval(DateTime currentHour, int interval, IntervalType intervalType)
    {
        return intervalType switch
        {
            IntervalType.Hourly => currentHour.AddHours(interval),
            IntervalType.Minutely => currentHour.AddMinutes(interval),
            IntervalType.Secondly => currentHour.AddSeconds(interval),
            _ => throw new ArgumentException("Invalid interval type")
        };
    }
}


