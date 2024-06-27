using SchedulerApp.Domain.Entities;

namespace SchedulerApplication.Services.Implementations;

public class ScheduleTypeRecurring : ScheduleTypeBase
{
    public override ScheduleOutput getNextExecutionTime(SchedulerConfiguration configuration)
    {
        ValidateConfiguration(configuration);

        DateTime nextExecutionTime = CalculateNextExecutionTime(configuration);
        return CreateScheduleOutput(configuration, nextExecutionTime);
    }

    private DateTime CalculateNextExecutionTime(SchedulerConfiguration configuration)
    {
        if (configuration.DaysInterval <= 0)
        {
            throw new ArgumentException("DaysInterval must be greater than 0", nameof(configuration.DaysInterval));
        }

        DateTime currentExecution = configuration.CurrentDate;
        DateTime endDate = configuration.TimeInterval.LimitEndDateTime == DateTime.MinValue ? DateTime.MaxValue : configuration.TimeInterval.LimitEndDateTime;

        while (currentExecution <= endDate)
        {
            if (currentExecution > configuration.CurrentDate)
            {
                return currentExecution;
            }

            currentExecution = currentExecution.AddDays(configuration.DaysInterval);
        }

        throw new InvalidOperationException("No valid execution time found within the provided interval.");
    }
}