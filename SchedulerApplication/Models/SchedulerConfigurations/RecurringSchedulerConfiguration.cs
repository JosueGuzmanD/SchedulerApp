using SchedulerApplication.Common.Enums;
using SchedulerApplication.ValueObjects;

namespace SchedulerApplication.Models.SchedulerConfigurations;

public class RecurringSchedulerConfiguration : SchedulerConfiguration
{
    public Frequency Frequency { get; set; }
    public int DaysInterval { get; set; }
    public List<DayOfWeek> DaysOfWeek { get; set; }
    public HourTimeRange HourTimeRange { get; set; }
    public int WeekInterval { get; set; }
}
