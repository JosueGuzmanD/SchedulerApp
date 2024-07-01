using SchedulerApplication.ValueObjects;

namespace SchedulerApplication.Services.Interfaces;

public interface IHourCalculatorService
{
    List<DateTime> CalculateHour(DateTime date, HourTimeRange timeRange);
}

