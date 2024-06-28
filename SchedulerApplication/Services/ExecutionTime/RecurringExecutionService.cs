using SchedulerApp.Domain.Entities;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.ExecutionTime;

public class RecurringExecutionService : IRecurringExecutionService
{
    public void ValidateConfiguration(SchedulerConfiguration configuration)
    {
        if (!configuration.IsEnabled)
        {
            throw new ArgumentException("Configuration must be enabled.");
        }
    }

    public List<DateTime> CalculateNextExecutionTimes(RecurringSchedulerConfiguration configuration)
    {
        ValidateConfiguration(configuration);

        List<DateTime> executionTimes = new List<DateTime>();
        var currentDate = configuration.CurrentDate;
        var endDate = configuration.TimeInterval.LimitEndDateTime ?? DateTime.MaxValue;
        int maxExecutions = 3;
        int executionCount = 0;

        while (currentDate <= endDate && executionCount < maxExecutions)
        {
            currentDate = currentDate.AddDays(configuration.DaysInterval);

            if (currentDate >= configuration.TimeInterval.LimitStartDateTime)
            {
                executionTimes.Add(currentDate);
                executionCount++;
            }
        }

        return executionTimes;
    }
}

