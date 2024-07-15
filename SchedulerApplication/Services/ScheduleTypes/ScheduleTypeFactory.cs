using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Models;

namespace SchedulerApplication.Services.ScheduleTypes;

public class ScheduleTypeFactory
{
    private readonly IDescriptionService _descriptionService;
    private readonly IExecutionTimeGenerator _executionTimeGenerator;

    public ScheduleTypeFactory(IDescriptionService descriptionService, IExecutionTimeGenerator executionTimeGenerator)
    {
        _descriptionService = descriptionService;
        _executionTimeGenerator = executionTimeGenerator;
    }

    public IScheduleType CreateScheduleType(SchedulerConfiguration configuration)
    {
        return configuration switch
        {
            OnceSchedulerConfiguration _ => new ScheduleTypeOnce(_descriptionService, _executionTimeGenerator),
            RecurringSchedulerConfiguration _ => new ScheduleTypeRecurring(_descriptionService, _executionTimeGenerator),
            _ => throw new ArgumentException("Unsupported configuration type.")
        };
    }
}
