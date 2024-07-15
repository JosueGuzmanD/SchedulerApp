using SchedulerApplication.Interfaces;
using SchedulerApplication.Models;
using SchedulerApplication.Models.SchedulerConfigurations;

namespace SchedulerApplication.Services.ScheduleTypes;

public class ScheduleTypeRecurring : ScheduleTypeBase<RecurringSchedulerConfiguration>
{
    private readonly IExecutionTimeGenerator _executionTimeGenerator;

    public ScheduleTypeRecurring(IDescriptionService descriptionService, IExecutionTimeGenerator executionTimeGenerator)
        : base(descriptionService, executionTimeGenerator)
    {
        _executionTimeGenerator = executionTimeGenerator;
    }

    protected override List<ScheduleOutput> CreateScheduleOutput(RecurringSchedulerConfiguration configuration)
    {
        var executionTimes = _executionTimeGenerator.GenerateExecutions(configuration);
        var outputs = new List<ScheduleOutput>();

        foreach (var time in executionTimes)
        {
            var description = _descriptionService.GenerateDescription(configuration, time);
            outputs.Add(new ScheduleOutput
            {
                Description = description,
                ExecutionTime = time
            });
        }

        return outputs;
    }
}
