using SchedulerApp.Domain.Entities;
using SchedulerApp.Domain.Interfaces;

namespace SchedulerApp.Application.Services;

public class ScheduleTypeRecurring : IScheduleType
{
    public ScheduleOutput getNextExecutionTime(SchedulerConfiguration configuration)
    {
        try
        {
            
            var output = new ScheduleOutput()
            {
                Description =
                    $"Occurs every day. Schedule will be used on {configuration.Date} at {configuration.Date.Hour} starting on {configuration.LimitStartDateTime.Date}"
            };

            DateTime currentExecution = configuration.LimitStartDateTime;
            DateTime endDate = configuration.LimitEndDateTime == DateTime.MinValue ? DateTime.MaxValue : configuration.LimitEndDateTime;
            bool isInfinite = configuration.LimitEndDateTime == DateTime.MinValue;

            int maxExecutions = 30;
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

            if (isInfinite && executionCount == maxExecutions)
            {
                output.Description += ". Execution times are capped at 30 entries.";
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