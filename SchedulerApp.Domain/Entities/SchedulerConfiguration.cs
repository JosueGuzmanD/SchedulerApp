using SchedulerApp.Domain.Enums;

namespace SchedulerApp.Domain.Entities
{
    public class SchedulerConfiguration
    {
        public SchedulerType Type { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime Date { get; set; }
        public SchedulerFrequency Frequency { get; set; }
        public int DaysFrequency { get; set; }
    }
}
