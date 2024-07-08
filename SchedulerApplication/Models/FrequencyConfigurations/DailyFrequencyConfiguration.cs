using SchedulerApplication.Models.SchedulerConfigurations;

namespace SchedulerApplication.Models.FrequencyConfigurations;

public class DailyFrequencyConfiguration : RecurringSchedulerConfiguration
{
    public bool OccursOnce { get; set; }
    public TimeSpan OnceAt { get; set; }
}

