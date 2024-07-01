using SchedulerApplication.Common.Enums;
using SchedulerApplication.Services.Interfaces;
using SchedulerApplication.ValueObjects;

namespace SchedulerApplication.Services.HourCalculator;

public class HourCalculatorService : IHourCalculatorService
{
    public List<DateTime> CalculateHour(DateTime date, HourTimeRange timeRange)
    {
        var hourList = new List<DateTime>();

        if (timeRange.HourlyFrequency == DailyHourFrequency.Once)
        {
            hourList.Add(date.Date.Add(timeRange.StartHour));
        }
        else if (timeRange.HourlyFrequency == DailyHourFrequency.Recurrent)
        {
            var start = date.Date.Add(timeRange.StartHour);
            var end = date.Date.Add(timeRange.EndHour);
            var interval = TimeSpan.FromHours(timeRange.HourlyInterval);
            for (var dt = start; dt <= end; dt = dt.Add(interval))
            {
                hourList.Add(dt);
            }
        }

        return hourList;
    }
}

