using SchedulerApplication.Models;

namespace SchedulerApplication.Interfaces;

public interface IDateCalculationStrategy
{
    List<DateTime> CalculateDates(SchedulerConfiguration config);
}

