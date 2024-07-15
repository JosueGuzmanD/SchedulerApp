using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Models;

namespace SchedulerApplication.Services.DateCalculator;

public class OnceDateCalculator : IDateCalculator
{
    public List<DateTime> CalculateDates(SchedulerConfiguration config)
    {
        var onceConfig = (OnceSchedulerConfiguration)config;
        return Enumerable.Range(0, 12).Select(i => onceConfig.ConfigurationDateTime.AddDays(i)).ToList();
    }
}
