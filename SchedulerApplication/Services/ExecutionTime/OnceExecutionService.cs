using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.ExecutionTime;

public class OnceExecutionService : IOnceExecutionService
{
    private readonly IConfigurationValidator _validator;

    public OnceExecutionService(IConfigurationValidator validator)
    {
        _validator = validator;
    }

    public DateTime CalculateNextExecutionTime(OnceSchedulerConfiguration configuration)
    {
        _validator.Validate(configuration);
        return configuration.CurrentDate;
    }
}

