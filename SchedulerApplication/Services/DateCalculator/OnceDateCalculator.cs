using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Models;

namespace SchedulerApplication.Services.DateCalculator;

public class OnceDateCalculator : IDateCalculator
{
    public List<DateTime> CalculateDates(SchedulerConfiguration configuration)
    {
        var onceConfig = configuration as OnceSchedulerConfiguration;
        if (onceConfig == null)
        {
            throw new ArgumentException("Invalid configuration type for OnceDateCalculator.");
        }

        if (onceConfig.ConfigurationDateTime < onceConfig.CurrentDate)
        {
            throw new ArgumentException("Configuration date time cannot be in the past.");
        }

        return new List<DateTime> { onceConfig.ConfigurationDateTime };
    }
}
