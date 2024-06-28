using SchedulerApp.Domain.Entities;

namespace SchedulerApplication.Services.Interfaces;

    public interface IDescriptionService
    {
      string GenerateDescription(SchedulerConfiguration configuration, DateTime executionTime);

    }

