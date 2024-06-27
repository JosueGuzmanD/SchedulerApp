using SchedulerApp.Domain.Common.Enums;
using SchedulerApplication.ValueObjects;

namespace SchedulerApp.Domain.Entities;

    public class SchedulerConfiguration
    {
        public SchedulerType Type { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CurrentDate { get; set; }
        public Frequency Frequency { get; set; }
        public int DaysInterval { get; set; } 
        public TimeInterval TimeInterval { get; set; }

}

