using SchedulerApp.Domain.Entities;

namespace SchedulerApplication.Services.Interfaces;

public interface IConfigurationValidator
{
    void Validate(SchedulerConfiguration configuration);
}

