using SchedulerApplication.ValueObjects;

namespace SchedulerApplication.Models.SchedulerConfigurations;

public abstract class RecurringSchedulerConfiguration : SchedulerConfiguration
{
    public HourTimeRange HourTimeRange { get; set; }
    public int HourlyInterval { get; set; }
}
