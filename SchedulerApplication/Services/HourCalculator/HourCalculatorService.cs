using SchedulerApplication.Common.Enums;
using SchedulerApplication.Services.Interfaces;
using SchedulerApplication.ValueObjects;

namespace SchedulerApplication.Services.HourCalculator;

public class HourCalculatorService : IHourCalculatorService
{
    public List<DateTime> CalculateHour(DateTime baseDate, HourTimeRange timeRange)
    {
        return timeRange.HourlyFrequency switch
        {
            DailyHourFrequency.Once => CalculateOnce(baseDate, timeRange),
            DailyHourFrequency.Recurrent => CalculateRecurrent(baseDate, timeRange),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static List<DateTime> CalculateOnce(DateTime baseDate, HourTimeRange timeRange) => new() { baseDate.Date.Add(timeRange.StartHour) };

    private static List<DateTime> CalculateRecurrent(DateTime baseDate, HourTimeRange timeRange)
    {
        var hourList = new List<DateTime>();
        var start = baseDate.Date.Add(timeRange.StartHour);
        var interval = TimeSpan.FromHours(timeRange.HourlyInterval);

        if (timeRange.StartHour > timeRange.EndHour)
        {
            CalculateCrossMidnight(hourList, start, baseDate, timeRange, interval);
        }
        else
        {
            CalculateSameDay(hourList, start, baseDate, timeRange, interval);
        }

        return hourList;
    }

    private static void CalculateCrossMidnight(List<DateTime> hourList, DateTime start, DateTime baseDate, HourTimeRange timeRange, TimeSpan interval)
    {
        var endToday = baseDate.Date.AddDays(1).Add(timeRange.EndHour);
        for (var dt = start; dt < endToday; dt = dt.Add(interval))
        {
            hourList.Add(dt);
        }
    }

    private static void CalculateSameDay(List<DateTime> hourList, DateTime start, DateTime baseDate, HourTimeRange timeRange, TimeSpan interval)
    {
        var end = baseDate.Date.Add(timeRange.EndHour);
        for (var dt = start; dt <= end; dt = dt.Add(interval))
        {
            hourList.Add(dt);
        }
    }

}