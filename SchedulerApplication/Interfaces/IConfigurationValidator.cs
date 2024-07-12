using SchedulerApplication.Models;

namespace SchedulerApplication.Interfaces;

public interface IConfigurationValidator
{
    void Validate(SchedulerConfiguration configuration);
}

