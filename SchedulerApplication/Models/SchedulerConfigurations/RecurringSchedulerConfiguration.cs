using SchedulerApplication.ValueObjects;

namespace SchedulerApplication.Models.SchedulerConfigurations;

public class RecurringSchedulerConfiguration : SchedulerConfiguration
{
    public HourTimeRange HourTimeRange { get; set; }
}
