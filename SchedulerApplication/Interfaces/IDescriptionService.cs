using SchedulerApplication.Models;

namespace SchedulerApplication.Interfaces;

public interface IDescriptionService
{
    string GenerateDescription(SchedulerConfiguration configuration, DateTime executionTime);
}

