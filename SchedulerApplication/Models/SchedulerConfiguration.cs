using SchedulerApplication.Common.Enums;

namespace SchedulerApplication.Models;

public abstract class SchedulerConfiguration
{ 
    public bool IsEnabled { get; set; }
    public DateTime CurrentDate { get; set; }
}

