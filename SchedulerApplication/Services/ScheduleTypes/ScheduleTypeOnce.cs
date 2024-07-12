using SchedulerApplication.Interfaces;
using SchedulerApplication.Models;
using SchedulerApplication.Models.SchedulerConfigurations;

namespace SchedulerApplication.Services.ScheduleTypes;

public class ScheduleTypeOnce : ScheduleTypeBase<OnceSchedulerConfiguration>
{
    private readonly IConfigurationValidator _validator;

    public ScheduleTypeOnce(IDescriptionService descriptionService, IExecutionTimeGenerator executionTimeGenerator, IConfigurationValidator validator)
        : base(descriptionService, executionTimeGenerator)
    {
        _validator = validator;
    }

    protected override List<ScheduleOutput> CreateScheduleOutput(OnceSchedulerConfiguration configuration)
    {
        _validator.Validate(configuration);
        var executionTime = _executionTimeGenerator.GenerateExecutions(configuration).First();
        var description = _descriptionService.GenerateDescription(configuration, executionTime);

        return
        [
            new ScheduleOutput()
            {
                Description = description,
                ExecutionTime = executionTime
            }
        ];
    }
}