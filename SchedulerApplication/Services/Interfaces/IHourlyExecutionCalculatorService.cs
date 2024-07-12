using SchedulerApplication.Models;

namespace SchedulerApplication.Services.Interfaces;

public interface IHourlyExecutionCalculatorService
{
    IEnumerable<DateTime> CalculateHourlyExecutions(SchedulerConfiguration config);

}

