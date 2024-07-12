using SchedulerApplication.Interfaces;
using SchedulerApplication.Models;

namespace SchedulerApplication.Services.ScheduleTypes;

public abstract class ScheduleTypeBase<TConfiguration> : IScheduleType where TConfiguration : SchedulerConfiguration
{
    protected readonly IDescriptionService _descriptionService;
    protected readonly IExecutionTimeGenerator _executionTimeGenerator;

    protected ScheduleTypeBase(IDescriptionService descriptionService, IExecutionTimeGenerator executionTimeGenerator)
    {
        _descriptionService = descriptionService;
        _executionTimeGenerator = executionTimeGenerator;
    }

    public List<ScheduleOutput> GetNextExecutionTimes(SchedulerConfiguration configuration)
    {
        return CreateScheduleOutput((TConfiguration)configuration);
    }

    protected abstract List<ScheduleOutput> CreateScheduleOutput(TConfiguration configuration);
}