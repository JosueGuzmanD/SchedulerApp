using SchedulerApp.Domain.Entities;
using SchedulerApp.Domain.Interfaces;

namespace SchedulerApp.Application.Services;

public class ScheduleTypeOnce: IScheduleType
{
    public ScheduleOutput getNextExecutionTime(SchedulerConfiguration configuration)
    {
        try
        {
            if (configuration.IsEnabled == false)
            {
                throw new InvalidOperationException("You must enable a configuration type.");
            }
            var output = new ScheduleOutput()
            {
                Description = $"Occurs {configuration.Type}. Schedule will be used on {configuration.Date:dd/MM/yyyy} at {configuration.Date.Hour} starting on {configuration.LimitStartDateTime:dd/MM/yyyy}",
                  
            };
            output.ExecutionTime.Add(configuration.Date.Date);

            return output;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}