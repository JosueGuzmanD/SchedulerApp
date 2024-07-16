using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.ValueObjects;

namespace SchedulerApplication.Services.HourCalculator;


public class HourCalculatorService 
{
    public List<DateTime> CalculateHours(List<DateTime> dates, HourTimeRange hourTimeRange, int hourlyInterval)
    {
        var results = new List<DateTime>();

        foreach (var date in dates)
        {
            var currentHour = date.Date.Add(hourTimeRange.StartHour);
            var endDateTime = date.Date.Add(hourTimeRange.EndHour);

            if (hourlyInterval == 0 && hourTimeRange.StartHour == hourTimeRange.EndHour)
            {
                // Interval is 0 and start hour is equal to end hour
                results.Add(currentHour);
                if (results.Count >= 12)
                    break;
                continue;
            }

            if (hourTimeRange.StartHour <= hourTimeRange.EndHour)
            {
                while (currentHour <= endDateTime && results.Count < 12)
                {
                    results.Add(currentHour);
                    currentHour = currentHour.AddHours(hourlyInterval);
                }
            }
            else
            {
                // Handle the case where the time range crosses midnight
                var nextDay = date.Date.AddDays(1);

                // Add hours until midnight
                while (currentHour.TimeOfDay < TimeSpan.FromHours(24) && results.Count < 12)
                {
                    results.Add(currentHour);
                    currentHour = currentHour.AddHours(hourlyInterval);
                }

                currentHour = nextDay.Date.Add(hourTimeRange.StartHour);

                // Add hours for the new day
                while (currentHour.TimeOfDay <= hourTimeRange.EndHour && results.Count < 12)
                {
                    results.Add(currentHour);
                    currentHour = currentHour.AddHours(hourlyInterval);
                }
            }

            if (results.Count >= 12)
                break;
        }

        return results;
    }
}


