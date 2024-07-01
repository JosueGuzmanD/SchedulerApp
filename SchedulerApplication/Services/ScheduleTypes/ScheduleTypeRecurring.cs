using SchedulerApplication.Models;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.ScheduleTypes;

public class ScheduleTypeRecurring : ScheduleTypeBase<RecurringSchedulerConfiguration>
{
    private readonly IRecurringExecutionService _recurringExecutionService;

    public ScheduleTypeRecurring(IDescriptionService descriptionService, IRecurringExecutionService recurringExecutionService)
        : base(descriptionService, recurringExecutionService)
    {
        _recurringExecutionService = recurringExecutionService;
    }

    protected override List<ScheduleOutput> CalculateNextExecutionTimes(RecurringSchedulerConfiguration configuration)
    {
        var executionTimes = _recurringExecutionService.CalculateNextExecutionTimes(configuration);
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