using SchedulerApplication.Common.Enums;
using SchedulerApplication.Interfaces;
using SchedulerApplication.Models;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using System.Globalization;

namespace SchedulerApplication.Services.Description;

using System.Globalization;
using System.Linq;

public class DescriptionService : IDescriptionService
{
    public string GenerateDescription(SchedulerConfiguration configuration, DateTime executionTime)
    {
        CultureManager.SetCulture(configuration.Culture);

        return configuration switch
        {
            DailyFrequencyConfiguration dailyConfig => GenerateDailyDescription(dailyConfig, executionTime),
            WeeklyFrequencyConfiguration weeklyConfig => GenerateWeeklyDescription(weeklyConfig, executionTime),
            SpecificDayMonthlySchedulerConfiguration specificDayConfig => GenerateSpecificDayDescription(specificDayConfig, executionTime),
            MonthlySchedulerConfiguration monthlyConfig => GenerateMonthlyDescription(monthlyConfig, executionTime),
            OnceSchedulerConfiguration onceConfig => GenerateOnceDescription(onceConfig, executionTime),
            _ => throw new ArgumentException(CultureManager.GetLocalizedString("ExecutionTimeGeneratorUnknownConfigurationExc"))
        };
    }

    private static string GenerateDailyDescription(DailyFrequencyConfiguration dailyConfig, DateTime executionTime)
    {
        var range = dailyConfig.HourTimeRange;
        var dateStr = CultureManager.FormatDate(executionTime);
        var timeStr = CultureManager.FormatTime(executionTime.TimeOfDay);
        var startStr = CultureManager.FormatDate(dailyConfig.CurrentDate);

        if (range.EndHour == TimeSpan.Zero)
        {
            return string.Format(CultureManager.GetLocalizedString("DailyDescriptionSingle"),
                range.StartHour.ToString(@"hh\:mm", CultureInfo.CurrentCulture),
                dateStr, timeStr, startStr);
        }

        return string.Format(CultureManager.GetLocalizedString("DailyDescription_Range"),
            range.StartHour.ToString(@"hh\:mm", CultureInfo.CurrentCulture),
            range.EndHour.ToString(@"hh\:mm", CultureInfo.CurrentCulture),
            dateStr, timeStr, startStr);
    }

    private static string GenerateWeeklyDescription(WeeklyFrequencyConfiguration weeklyConfig, DateTime executionTime)
    {
        var range = weeklyConfig.HourTimeRange;
        var dateStr = CultureManager.FormatDate(executionTime);
        var timeStr = CultureManager.FormatTime(executionTime.TimeOfDay);
        var startStr = CultureManager.FormatDate(weeklyConfig.CurrentDate);
        var daysOfWeek = string.Join(", ", weeklyConfig.DaysOfWeek.Select(d => CultureManager.GetLocalizedString(d.ToString())));

        return string.Format(CultureManager.GetLocalizedString("WeeklyDescription"),
            weeklyConfig.WeekInterval,
            daysOfWeek,
            dateStr,
            timeStr,
            startStr);
    }

    private static string GenerateMonthlyDescription(MonthlySchedulerConfiguration monthlyConfig, DateTime executionTime)
    {
        var dateStr = CultureManager.FormatDate(executionTime.Date);
        var timeStr = CultureManager.FormatTime(executionTime.TimeOfDay);
        var startStr = CultureManager.FormatDate(monthlyConfig.CurrentDate.Date);

        if (monthlyConfig.WeekOption == WeekOptions.AnyDay)
        {
            return string.Format(CultureManager.GetLocalizedString("MonthlyDescription_AnyDay"),
                executionTime.Day,
                monthlyConfig.MonthFrequency,
                dateStr,
                timeStr,
                startStr);
        }

        var dayOption = monthlyConfig.DayOptions switch
        {
            DayOptions.First => CultureManager.GetLocalizedString("First"),
            DayOptions.Second => CultureManager.GetLocalizedString("Second"),
            DayOptions.Third => CultureManager.GetLocalizedString("Third"),
            DayOptions.Fourth => CultureManager.GetLocalizedString("Fourth"),
            DayOptions.Last => CultureManager.GetLocalizedString("Last"),
            _ => throw new ArgumentOutOfRangeException()
        };

        var weekOption = monthlyConfig.WeekOption switch
        {
            WeekOptions.Weekday => CultureManager.GetLocalizedString("Weekday"),
            WeekOptions.WeekendDay => CultureManager.GetLocalizedString("WeekendDay"),
            _ => CultureManager.GetLocalizedString(monthlyConfig.WeekOption.ToString())
        };

        return string.Format(CultureManager.GetLocalizedString("MonthlyDescription_DayOption"),
            dayOption,
            weekOption,
            monthlyConfig.MonthFrequency,
            dateStr,
            timeStr,
            startStr);
    }

    private static string GenerateSpecificDayDescription(SpecificDayMonthlySchedulerConfiguration specificDayConfig, DateTime executionTime)
    {
        var dateStr = CultureManager.FormatDate(executionTime);
        var timeStr = CultureManager.FormatTime(executionTime.TimeOfDay);
        var startStr = CultureManager.FormatDate(specificDayConfig.CurrentDate);

        return string.Format(CultureManager.GetLocalizedString("MonthlyDescription_SpecificDay"),
            specificDayConfig.SpecificDay,
            specificDayConfig.MonthFrequency,
            dateStr,
            timeStr,
            startStr);
    }

    protected virtual string GenerateOnceDescription(OnceSchedulerConfiguration onceConfig, DateTime executionTime)
    {
        var dateStr = CultureManager.FormatDate(executionTime);
        var timeStr = CultureManager.FormatTime(executionTime.TimeOfDay);
        var startStr = CultureManager.FormatDate(onceConfig.CurrentDate);

        return string.Format(CultureManager.GetLocalizedString("OnceDescription"),
            dateStr,
            timeStr,
            startStr);
    }
}
