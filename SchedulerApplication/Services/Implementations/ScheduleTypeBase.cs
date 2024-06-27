using SchedulerApp.Domain.Common.Enums;
using SchedulerApp.Domain.Entities;
using SchedulerApp.Domain.Interfaces;

namespace SchedulerApplication.Services.Implementations;

public abstract class ScheduleTypeBase : IScheduleType
{
    public abstract ScheduleOutput getNextExecutionTime(SchedulerConfiguration configuration);

    protected void ValidateConfiguration(SchedulerConfiguration configuration)
    {
        if (!configuration.IsEnabled)
        {
            throw new ArgumentException("Configuration must be enabled.");
        }
    }

    protected string GenerateDescription(SchedulerConfiguration configuration, DateTime executionTime)
    {
        string intervalDescription = configuration.Type == SchedulerType.Once
            ? "Occurs Once"
            : configuration.DaysInterval == 1
                ? "Occurs every day"
                : $"Occurs every {configuration.DaysInterval} days";

        return $"{intervalDescription}. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.TimeInterval.LimitStartDateTime:dd/MM/yyyy}.";
    }

    protected ScheduleOutput CreateScheduleOutput(SchedulerConfiguration configuration, DateTime executionTime)
    {
        return new ScheduleOutput
        {
            Description = GenerateDescription(configuration, executionTime),
            ExecutionTime = executionTime
        };
    }
}
        


