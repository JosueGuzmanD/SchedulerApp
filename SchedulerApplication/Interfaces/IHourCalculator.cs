using SchedulerApplication.Models.ValueObjects;

namespace SchedulerApplication.Interfaces;

    public interface IHourCalculator
    {
        List<DateTime> CalculateHours(List<DateTime> dates, HourTimeRange hourTimeRange, int hourlyInterval);
    }

