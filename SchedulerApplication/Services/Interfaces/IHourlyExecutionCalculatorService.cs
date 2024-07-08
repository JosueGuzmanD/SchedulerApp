using SchedulerApplication.Models.SchedulerConfigurations;

namespace SchedulerApplication.Services.Interfaces;

public interface IHourlyExecutionCalculatorService
{
    List<DateTime> CalculateHourlyExecutions(RecurringSchedulerConfiguration config);

}

