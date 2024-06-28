using SchedulerApp.Domain.Entities;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.ScheduleTypes;

public class ScheduleTypeOnce : ScheduleTypeBase<OnceSchedulerConfiguration>
{
    private readonly IOnceExecutionService _onceExecutionService;
    private readonly IConfigurationValidator _validator;

    public ScheduleTypeOnce(IDescriptionService descriptionService, IOnceExecutionService onceExecutionService, IConfigurationValidator validator)
        : base(descriptionService, onceExecutionService)
    {
        _onceExecutionService = onceExecutionService;
        _validator = validator;
    }

    protected override List<ScheduleOutput> CalculateNextExecutionTimes(OnceSchedulerConfiguration configuration)
    {
        _validator.Validate(configuration);
        var executionTime = _onceExecutionService.CalculateNextExecutionTime(configuration);
        var description = _descriptionService.GenerateDescription(configuration, executionTime);

        return new List<ScheduleOutput>
        {
            new ScheduleOutput
            {
                Description = description,
                ExecutionTime = executionTime
            }
        };
    }
}