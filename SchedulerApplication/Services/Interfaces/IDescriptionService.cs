using SchedulerApplication.Models;

namespace SchedulerApplication.Services.Interfaces;

public interface IDescriptionService
{
    string GenerateDescription(SchedulerConfiguration configuration, DateTime executionTime);
}

