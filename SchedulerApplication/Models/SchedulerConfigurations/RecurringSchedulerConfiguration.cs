using SchedulerApplication.Common.Enums;
using SchedulerApplication.Models.ValueObjects;

namespace SchedulerApplication.Models.SchedulerConfigurations;

public abstract class RecurringSchedulerConfiguration : SchedulerConfiguration
{
    public HourTimeRange HourTimeRange { get; set; }
    public IntervalType IntervalType { get; set; }
    public int Interval { get; set; }

}
