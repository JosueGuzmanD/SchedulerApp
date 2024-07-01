using SchedulerApplication.Models;

namespace SchedulerApplication.Services.Interfaces;

public interface IConfigurationValidator
{
    void Validate(SchedulerConfiguration configuration);
}

