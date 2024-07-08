using SchedulerApplication.Common.Enums;
using SchedulerApplication.Models;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.Description;

public class DescriptionService : IDescriptionService
{
    public string GenerateDescription(SchedulerConfiguration configuration, DateTime executionTime)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        if (configuration is DailyFrequencyConfiguration dailyConfig)
        {
            if (dailyConfig.HourTimeRange == null)
            {
                throw new ArgumentNullException(nameof(dailyConfig.HourTimeRange));
            }

            if (dailyConfig.HourTimeRange.HourlyFrequency == DailyHourFrequency.Once)
            {
                return $"Occurs once at {dailyConfig.HourTimeRange.StartHour:hh\\:mm}. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.";
            }
            else
            {
                return $"Occurs every day from {dailyConfig.HourTimeRange.StartHour:hh\\:mm} to {dailyConfig.HourTimeRange.EndHour:hh\\:mm} every {dailyConfig.HourTimeRange.HourlyInterval} hour(s). Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.";
            }
        }

        if (configuration is WeeklyFrequencyConfiguration weeklyConfig)
        {
            if (weeklyConfig.HourTimeRange == null)
            {
                throw new ArgumentNullException(nameof(weeklyConfig.HourTimeRange));
            }

            if (weeklyConfig.DaysOfWeek == null || !weeklyConfig.DaysOfWeek.Any())
            {
                throw new ArgumentNullException(nameof(weeklyConfig.DaysOfWeek));
            }

            var daysOfWeek = string.Join(", ", weeklyConfig.DaysOfWeek);
            return $"Occurs every {weeklyConfig.WeekInterval} week(s) on {daysOfWeek}. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.";
        }

        if (configuration is OnceSchedulerConfiguration)
        {
            return $"Occurs once. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.";
        }

        throw new NotSupportedException("Unsupported configuration type.");
    }
}
