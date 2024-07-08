using SchedulerApplication.Models;
using SchedulerApplication.Models.SchedulerConfigurations;

namespace SchedulerApplication.Services.Interfaces;

public interface IHourlyExecutionCalculatorService
{
    List<DateTime> CalculateHourlyExecutions(SchedulerConfiguration config);

}

