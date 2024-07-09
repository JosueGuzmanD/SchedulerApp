using SchedulerApplication.Models;
using SchedulerApplication.Models.SchedulerConfigurations;

namespace SchedulerApplication.Services.Interfaces;

public interface IHourlyExecutionCalculatorService
{
    IEnumerable<DateTime> CalculateHourlyExecutions(SchedulerConfiguration config);

}

