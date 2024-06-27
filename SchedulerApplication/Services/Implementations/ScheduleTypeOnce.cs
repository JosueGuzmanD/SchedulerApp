using SchedulerApp.Domain.Entities;

namespace SchedulerApplication.Services.Implementations;

public class ScheduleTypeOnce : ScheduleTypeBase
{
    public override ScheduleOutput getNextExecutionTime(SchedulerConfiguration configuration)
    {
        ValidateConfiguration(configuration);
        return CreateScheduleOutput(configuration, configuration.CurrentDate);
    }
}