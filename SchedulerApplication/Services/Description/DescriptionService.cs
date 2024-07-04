using SchedulerApplication.Models;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.Description;

public class DescriptionService : IDescriptionService
{
    public string GenerateDescription(SchedulerConfiguration configuration, DateTime executionTime)
    {
        if (configuration is RecurringSchedulerConfiguration { DaysInterval: 0 })
        {
            throw new IndexOutOfRangeException("Days interval cannot be 0");
        }

        string intervalDescription= string.Empty;

        if (configuration is OnceSchedulerConfiguration)
        {
            intervalDescription = "Occurs Once";
        }
        else if (configuration is RecurringSchedulerConfiguration recurringConfig)
        {
            if (recurringConfig.DaysInterval == 1)
            {
                intervalDescription = "Occurs every day";
            }
            else
            {
                intervalDescription = $"Occurs every {recurringConfig.DaysInterval} days";
            }
        }
    

        return $"{intervalDescription}. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.TimeInterval.LimitStartDateTime:dd/MM/yyyy}.";
    }
}
