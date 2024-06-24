using SchedulerApp.Domain.Common.Enums;

namespace SchedulerApp.Application.Dtos
{
    public class SchedulerConfigurationDto
    {
        public SchedulerType Type { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime Date { get; set; }
        public SchedulerFrequency Frequency { get; set; }
        public int DaysFrequency { get; set; }
    }
}
