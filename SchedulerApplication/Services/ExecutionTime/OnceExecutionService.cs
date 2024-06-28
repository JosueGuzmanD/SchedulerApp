using SchedulerApp.Domain.Entities;
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

    public void ValidateConfiguration(SchedulerConfiguration configuration)
    {
        _validator.Validate(configuration);
    }

    public DateTime CalculateNextExecutionTime(OnceSchedulerConfiguration configuration)
    {
        ValidateConfiguration(configuration);
        return configuration.CurrentDate;
    }
}

