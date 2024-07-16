using SchedulerApplication.Models;

namespace SchedulerApplication.Interfaces;

// Separates the logic of generating descriptions from the scheduling and calculation logic.
// Allows implementing different strategies for generating descriptions.
public interface IDescriptionService
{
    string GenerateDescription(SchedulerConfiguration configuration, DateTime executionTime);
}

