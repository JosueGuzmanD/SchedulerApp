using SchedulerApplication.Common.Enums;
using SchedulerApplication.Models.SchedulerConfigurations;

namespace SchedulerApplication.Models.FrequencyConfigurations;

public class MonthlySchedulerConfiguration: RecurringSchedulerConfiguration
{
    public WeekOptions WeekOption { get; set; }
    public int MonthFrequency { get; set; }
    public DayOptions DayOptions { get; set; }
    public DateTime CurrentDate { get; set; }
    public int MaxExecutions { get; set; }
}
    

