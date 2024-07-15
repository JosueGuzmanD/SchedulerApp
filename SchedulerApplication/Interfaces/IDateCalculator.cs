using SchedulerApplication.Models;

namespace SchedulerApplication.Interfaces;

    public interface IDateCalculator
    {
        List<DateTime> CalculateDates(SchedulerConfiguration config);
    }

