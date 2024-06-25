using SchedulerApp.Domain.Common.Enums;

namespace SchedulerApp.Domain.Entities;

    public class SchedulerConfiguration
    {
        public SchedulerType Type { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime StartDate { get; set; }
        public SchedulerFrequency Frequency { get; set; }
        public int DaysInterval { get; set; }
        public DateTime LimitStartDateTime { get; set; }
        public DateTime LimitEndDateTime { get; set; }
    }

