using SchedulerApplication.Models.SchedulerConfigurations;

namespace SchedulerApplication.Models.FrequencyConfigurations;

public class WeeklyFrequencyConfiguration : RecurringSchedulerConfiguration
{
    public int WeekInterval { get; set; } = 1;
    public List<DayOfWeek> DaysOfWeek { get; set; } = new List<DayOfWeek>();
}

