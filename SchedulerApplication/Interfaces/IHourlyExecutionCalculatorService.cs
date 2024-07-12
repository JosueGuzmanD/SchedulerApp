using SchedulerApplication.Models;

namespace SchedulerApplication.Interfaces;

public interface IHourlyExecutionCalculatorService
{
    IEnumerable<DateTime> CalculateHourlyExecutions(SchedulerConfiguration config);

}

