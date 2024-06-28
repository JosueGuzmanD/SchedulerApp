using SchedulerApp.Domain.Common.Enums;

namespace SchedulerApp.Domain.Entities;

    public abstract class SchedulerConfiguration
    {
        public SchedulerType Type { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CurrentDate { get; set; }
    }

