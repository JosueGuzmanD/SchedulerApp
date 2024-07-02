using SchedulerApplication.Common.Enums;
using SchedulerApplication.Services.Interfaces;
using SchedulerApplication.ValueObjects;

namespace SchedulerApplication.Services.HourCalculator;

public class HourCalculatorService : IHourCalculatorService
{
    public List<DateTime> CalculateHour(DateTime baseDate, HourTimeRange timeRange)
    {
        var hourList = new List<DateTime>();

        switch (timeRange.HourlyFrequency)
        {
            case DailyHourFrequency.Once:
                hourList.Add(baseDate.Date.Add(timeRange.StartHour));
                break;

            case DailyHourFrequency.Recurrent:
            {
                var start = baseDate.Date.Add(timeRange.StartHour);
                var end = baseDate.Date.Add(timeRange.EndHour);
                var interval = TimeSpan.FromHours(timeRange.HourlyInterval);
                for (var dt = start; dt <= end; dt = dt.Add(interval))
                {
                    hourList.Add(dt);
                }

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return hourList;
    }
}

