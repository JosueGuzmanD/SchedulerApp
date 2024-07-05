using SchedulerApplication.Models.SchedulerConfigurations;

namespace SchedulerApplication.Models.FrequencyConfigurations;

public class DailyFrequencyConfiguration : RecurringSchedulerConfiguration
{
    public int DaysInterval { get; set; }
}

