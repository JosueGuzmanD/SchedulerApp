using SchedulerApplication.Models;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.ScheduleTypes;

public abstract class ScheduleTypeBase<TConfiguration> : IScheduleType where TConfiguration : SchedulerConfiguration
{
    protected readonly IDescriptionService _descriptionService;
    protected readonly ISchedulerExecutionService _executionTimeService;

    protected ScheduleTypeBase(IDescriptionService descriptionService, ISchedulerExecutionService executionTimeService)
    {
        _descriptionService = descriptionService;
        _executionTimeService = executionTimeService;
    }

    public List<ScheduleOutput> GetNextExecutionTimes(SchedulerConfiguration configuration)
    {
        _executionTimeService.ValidateConfiguration(configuration);
        return CalculateNextExecutionTimes((TConfiguration)configuration);
    }

    protected abstract List<ScheduleOutput> CalculateNextExecutionTimes(TConfiguration configuration);
}