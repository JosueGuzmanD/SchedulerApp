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
                Description = $"Occurs {configuration.Type}. Schedule will be used on {configuration.StartDate:dd/MM/yyyy} at {configuration.StartDate.Hour} starting on {configuration.LimitStartDateTime:dd/MM/yyyy}",
                  
            };
            output.ExecutionTime.Add(configuration.StartDate.Date);

            return output;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}