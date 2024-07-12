using SchedulerApplication.Models.SchedulerConfigurations;

namespace SchedulerApplication.Models.FrequencyConfigurations;

public class WeeklyFrequencyConfiguration : RecurringSchedulerConfiguration
{
    public int WeekInterval { get; set; } 
    public List<DayOfWeek> DaysOfWeek { get; set; } 
}

