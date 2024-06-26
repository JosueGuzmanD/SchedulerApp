using SchedulerApp.Domain.Entities;
using SchedulerApp.Domain.Interfaces;

namespace SchedulerApp.Application.Services;

public class ScheduleTypeRecurring : IScheduleType
{
    public ScheduleOutput getNextExecutionTime(SchedulerConfiguration configuration)
    {
        try
        {
            if (!configuration.IsEnabled)
            {
                throw new InvalidOperationException("You must enable a configuration type.");
            }

            var output = new ScheduleOutput()
            {
                Description = $"Occurs every day. Schedule will be used on {configuration.StartDate:dd/MM/yy} at {configuration.StartDate.Hour} starting on {configuration.LimitStartDateTime.Date:dd/MM/yy}"
            };

            DateTime currentExecution = configuration.StartDate;
            DateTime endDate = configuration.LimitEndDateTime == DateTime.MinValue ? DateTime.MaxValue : configuration.LimitEndDateTime;

           const int maxExecutions = 3;
            int executionCount = 0;

            while (currentExecution <= endDate && executionCount < maxExecutions)
            {
                if (currentExecution >= configuration.LimitStartDateTime)
                {
                    output.ExecutionTime.Add(currentExecution);
                    executionCount++;
                }

                currentExecution = currentExecution.AddDays(configuration.DaysInterval);
            }

            if (configuration.LimitEndDateTime == DateTime.MinValue && executionCount == maxExecutions)
            {
                output.Description += ". Execution times are capped at 3 entries.";
            }

            return output;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}