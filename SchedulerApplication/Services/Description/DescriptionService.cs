using SchedulerApplication.Models;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.Description;

public class DescriptionService : IDescriptionService
{
    public string GenerateDescription(SchedulerConfiguration configuration, DateTime executionTime)
    {
        string intervalDescription = configuration is OnceSchedulerConfiguration
            ? "Occurs Once"
            : ((RecurringSchedulerConfiguration)configuration).DaysInterval == 1
                ? "Occurs every day"
                : $"Occurs every {((RecurringSchedulerConfiguration)configuration).DaysInterval} days";

        return
            $"{intervalDescription}. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.";
    }
}
