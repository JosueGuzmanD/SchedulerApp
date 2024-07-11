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
        return configuration switch
        {
            DailyFrequencyConfiguration dailyConfig => GenerateDailyDescription(dailyConfig, executionTime),
            WeeklyFrequencyConfiguration weeklyConfig => GenerateWeeklyDescription(weeklyConfig, executionTime),
            OnceSchedulerConfiguration onceConfig => GenerateOnceDescription(onceConfig, executionTime),
        };
    }

    private string GenerateDailyDescription(DailyFrequencyConfiguration dailyConfig, DateTime executionTime)
    {
        if (dailyConfig.HourTimeRange == null)
        {
            throw new ArgumentNullException();
        }

        var range = dailyConfig.HourTimeRange;
        return range.HourlyFrequency == DailyHourFrequency.Once
            ? $@"Occurs once at {range.StartHour:hh\:mm}. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {dailyConfig.CurrentDate:dd/MM/yyyy}."
            : $@"Occurs every day from {range.StartHour:hh\:mm} to {range.EndHour:hh\:mm} every {range.HourlyInterval} hour(s). Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {dailyConfig.CurrentDate:dd/MM/yyyy}.";
    }

    private string GenerateWeeklyDescription(WeeklyFrequencyConfiguration weeklyConfig, DateTime executionTime)
    {
        if (weeklyConfig.HourTimeRange == null)
        {
            throw new ArgumentNullException();
        }

        if (weeklyConfig.DaysOfWeek == null || !weeklyConfig.DaysOfWeek.Any())
        {
            throw new ArgumentNullException();
        }

        var daysOfWeek = string.Join(", ", weeklyConfig.DaysOfWeek);
        return $"Occurs every {weeklyConfig.WeekInterval} week(s) on {daysOfWeek}. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {weeklyConfig.CurrentDate:dd/MM/yyyy}.";
    }

    private string GenerateOnceDescription(OnceSchedulerConfiguration onceConfig, DateTime executionTime)
    {
        return $"Occurs once. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {onceConfig.CurrentDate:dd/MM/yyyy}.";
    }
}
