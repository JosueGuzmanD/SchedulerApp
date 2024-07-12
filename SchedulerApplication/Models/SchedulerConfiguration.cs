using SchedulerApplication.Models.ValueObjects;

namespace SchedulerApplication.Models;

public abstract class SchedulerConfiguration
{ 
    public bool IsEnabled { get; set; }
    public DateTime CurrentDate { get; set; }
    public LimitsTimeInterval Limits { get; set; }

}

