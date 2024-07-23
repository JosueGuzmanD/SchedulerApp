using SchedulerApplication.Common.Enums;
using SchedulerApplication.Interfaces;
using SchedulerApplication.Models;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;

namespace SchedulerApplication.Services.Description;

public class DescriptionService : IDescriptionService
{
    public string GenerateDescription(SchedulerConfiguration configuration, DateTime executionTime)
    {
        return configuration switch
        {
            DailyFrequencyConfiguration dailyConfig => GenerateDailyDescription(dailyConfig, executionTime),
            WeeklyFrequencyConfiguration weeklyConfig => GenerateWeeklyDescription(weeklyConfig, executionTime),
            SpecificDayMonthlySchedulerConfiguration specificDayConfig => GenerateSpecificDayDescription(specificDayConfig, executionTime),
            MonthlySchedulerConfiguration monthlyConfig => GenerateMonthlyDescription(monthlyConfig, executionTime),
            OnceSchedulerConfiguration onceConfig => GenerateOnceDescription(onceConfig, executionTime),
            _ => throw new ArgumentException("Unknown configuration type")
        };
    }

    private static string GenerateDailyDescription(DailyFrequencyConfiguration dailyConfig, DateTime executionTime)
    {
        if (dailyConfig.HourTimeRange == null)
        {
            throw new ArgumentNullException(nameof(dailyConfig.HourTimeRange));
        }

        var range = dailyConfig.HourTimeRange;
        return range.EndHour == TimeSpan.Zero
            ? $@"Occurs once at {range.StartHour:hh\:mm}. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {dailyConfig.CurrentDate:dd/MM/yyyy}."
            : $@"Occurs every day from {range.StartHour:hh\:mm} to {range.EndHour:hh\:mm}. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {dailyConfig.CurrentDate:dd/MM/yyyy}.";
    }

    private static string GenerateWeeklyDescription(WeeklyFrequencyConfiguration weeklyConfig, DateTime executionTime)
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
        return $"Occurs every {weeklyConfig.WeekInterval} week(s) on {daysOfWeek}. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {weeklyConfig.CurrentDate:dd/MM/yyyy}.";
    }

    private static string GenerateMonthlyDescription(MonthlySchedulerConfiguration monthlyConfig, DateTime executionTime)
    {
        if (monthlyConfig.WeekOption == WeekOptions.AnyDay)
        {
            return $"Occurs on day {executionTime.Day} of every {monthlyConfig.MonthFrequency} month(s). Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {monthlyConfig.CurrentDate:dd/MM/yyyy}.";
        }

        var dayOption = monthlyConfig.DayOptions switch
        {
            DayOptions.First => "first",
            DayOptions.Second => "second",
            DayOptions.Third => "third",
            DayOptions.Fourth => "fourth",
            DayOptions.Last => "last",
            _ => throw new ArgumentOutOfRangeException()
        };

        var weekOption = monthlyConfig.WeekOption switch
        {
            WeekOptions.Weekday => "weekday",
            WeekOptions.WeekendDay => "weekend day",
            _ => monthlyConfig.WeekOption.ToString()
        };

        return $"Occurs on the {dayOption} {weekOption} of every {monthlyConfig.MonthFrequency} month(s). Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {monthlyConfig.CurrentDate:dd/MM/yyyy}.";
    }

    private static string GenerateSpecificDayDescription(SpecificDayMonthlySchedulerConfiguration specificDayConfig, DateTime executionTime)
    {
        return $"Occurs on day {specificDayConfig.SpecificDay} of every {specificDayConfig.MonthFrequency} month(s). Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {specificDayConfig.CurrentDate:dd/MM/yyyy}.";
    }

    protected virtual string GenerateOnceDescription(OnceSchedulerConfiguration onceConfig, DateTime executionTime) => $"Occurs once. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {onceConfig.CurrentDate:dd/MM/yyyy}.";
}
