using SchedulerApplication.Models;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.Description;

public class DescriptionService : IDescriptionService
{
    public string GenerateDescription(SchedulerConfiguration configuration, DateTime executionTime)
    {
        if (configuration is DailyFrequencyConfiguration { DaysInterval: 0 } or WeeklyFrequencyConfiguration { DaysInterval: 0 })
        {
            throw new IndexOutOfRangeException("Days interval cannot be 0");
        }

        var intervalDescription = string.Empty;

        switch (configuration)
        {
            case OnceSchedulerConfiguration:
                intervalDescription = "Occurs Once";
                break;
            case DailyFrequencyConfiguration { DaysInterval: 1 }:
                intervalDescription = "Occurs every day";
                break;
            case DailyFrequencyConfiguration dailyConfig:
                intervalDescription = $"Occurs every {dailyConfig.DaysInterval} days";
                break;
            case WeeklyFrequencyConfiguration { DaysInterval: 1 }:
                intervalDescription = "Occurs every week";
                break;
            case WeeklyFrequencyConfiguration weeklyConfig:
                intervalDescription = $"Occurs every {weeklyConfig.DaysInterval} weeks";
                break;
        }

        return $"{intervalDescription}. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.TimeInterval.LimitStartDateTime:dd/MM/yyyy}.";
    }
}
