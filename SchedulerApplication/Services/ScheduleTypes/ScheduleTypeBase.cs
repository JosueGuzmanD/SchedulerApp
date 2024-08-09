using SchedulerApplication.Interfaces;
using SchedulerApplication.Models;

namespace SchedulerApplication.Services.ScheduleTypes;

public abstract class ScheduleTypeBase<TConfiguration> : IScheduleType where TConfiguration : SchedulerConfiguration
{
    protected readonly IDescriptionService DescriptionService;
    protected readonly IExecutionTimeGenerator ExecutionTimeGenerator;

    protected ScheduleTypeBase(IDescriptionService descriptionService, IExecutionTimeGenerator executionTimeGenerator)
    {
        DescriptionService = descriptionService;
        ExecutionTimeGenerator = executionTimeGenerator;
    }

    public List<ScheduleOutput> GetNextExecutionTimes(SchedulerConfiguration configuration)
    {
        return CreateScheduleOutput((TConfiguration)configuration);
    }

    protected abstract List<ScheduleOutput> CreateScheduleOutput(TConfiguration configuration);
}