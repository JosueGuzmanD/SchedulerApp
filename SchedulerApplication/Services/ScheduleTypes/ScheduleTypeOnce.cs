using SchedulerApplication.Interfaces;
using SchedulerApplication.Models;
using SchedulerApplication.Models.SchedulerConfigurations;

namespace SchedulerApplication.Services.ScheduleTypes;

public class ScheduleTypeOnce : ScheduleTypeBase<OnceSchedulerConfiguration>
{
    private readonly int _maxExecutions;

    public ScheduleTypeOnce(IDescriptionService descriptionService, IExecutionTimeGenerator executionTimeGenerator, int maxExecutions)
        : base(descriptionService, executionTimeGenerator)
    {
        _maxExecutions = maxExecutions;
    }

    protected override List<ScheduleOutput> CreateScheduleOutput(OnceSchedulerConfiguration configuration)
    {
        var executionTimes = ExecutionTimeGenerator.GenerateExecutions(configuration, _maxExecutions);
        var outputs = new List<ScheduleOutput>();

        foreach (var time in executionTimes)
        {
            var description = DescriptionService.GenerateDescription(configuration, time);
            outputs.Add(new ScheduleOutput
            {
                Description = description,
                ExecutionTime = time
            });
        }

        return outputs;
    }
}
