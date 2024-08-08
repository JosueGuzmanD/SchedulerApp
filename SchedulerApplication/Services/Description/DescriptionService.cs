using System.Globalization;
using Microsoft.Extensions.Localization;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Models;

public class DescriptionService : IDescriptionService
{
    private readonly IStringLocalizer _localizer;

    public DescriptionService(IStringLocalizer localizer)
    {
        _localizer = localizer;
    }

    public string GenerateDescription(SchedulerConfiguration configuration, DateTime executionTime)
    {
        SetCulture(configuration.Culture);

        return configuration switch
        {
            DailyFrequencyConfiguration dailyConfig => GenerateDailyDescription(dailyConfig, executionTime),
            WeeklyFrequencyConfiguration weeklyConfig => GenerateWeeklyDescription(weeklyConfig, executionTime),
            SpecificDayMonthlySchedulerConfiguration specificDayConfig => GenerateSpecificDayDescription(specificDayConfig, executionTime),
            MonthlySchedulerConfiguration monthlyConfig => GenerateMonthlyDescription(monthlyConfig, executionTime),
            OnceSchedulerConfiguration onceConfig => GenerateOnceDescription(onceConfig, executionTime),
            _ => throw new ArgumentException(_localizer["ExecutionTimeGeneratorUnknownConfigurationExc"].Value)
        };
    }
    private static void SetCulture(CultureOptions culture)
    {
        var cultureName = culture.ToString().Replace("_", "-");
        CultureInfo.CurrentCulture = new CultureInfo(cultureName);
        CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
    }
    private string GenerateDailyDescription(DailyFrequencyConfiguration dailyConfig, DateTime executionTime)
    {
        var range = dailyConfig.HourTimeRange;
        var dateStr = executionTime.ToString("d", CultureInfo.CurrentCulture);
        var timeStr = executionTime.ToString("t", CultureInfo.CurrentCulture);
        var startStr = dailyConfig.CurrentDate.ToString("d", CultureInfo.CurrentCulture);

        if (range.EndHour == TimeSpan.Zero)
        {
            return string.Format(_localizer["DailyDescriptionSingle"].Value, range.StartHour.ToString(@"hh\:mm", CultureInfo.CurrentCulture), dateStr, timeStr, startStr);
        }

        return string.Format(_localizer["DailyDescription_Range"].Value, range.StartHour.ToString(@"hh\:mm", CultureInfo.CurrentCulture), range.EndHour.ToString(@"hh\:mm", CultureInfo.CurrentCulture), dateStr, timeStr, startStr);
    }

    private string GenerateWeeklyDescription(WeeklyFrequencyConfiguration weeklyConfig, DateTime executionTime)
    {
        var range = weeklyConfig.HourTimeRange;
        var dateStr = executionTime.ToString("d", CultureInfo.CurrentCulture);
        var timeStr = executionTime.ToString("t", CultureInfo.CurrentCulture);
        var startStr = weeklyConfig.CurrentDate.ToString("d", CultureInfo.CurrentCulture);
        var daysOfWeek = string.Join(", ", weeklyConfig.DaysOfWeek.Select(d => _localizer[d.ToString()].Value));

        return string.Format(_localizer["WeeklyDescription"].Value, weeklyConfig.WeekInterval, daysOfWeek, dateStr, timeStr, startStr);
    }

    private string GenerateMonthlyDescription(MonthlySchedulerConfiguration monthlyConfig, DateTime executionTime)
    {
        var dateStr = executionTime.ToString("d", CultureInfo.CurrentCulture);
        var timeStr = executionTime.ToString("t", CultureInfo.CurrentCulture);
        var startStr = monthlyConfig.CurrentDate.ToString("d", CultureInfo.CurrentCulture);

        if (monthlyConfig.WeekOption == WeekOptions.AnyDay)
        {
            return string.Format(_localizer["MonthlyDescription_AnyDay"].Value, executionTime.Day, monthlyConfig.MonthFrequency, dateStr, timeStr, startStr);
        }

        var dayOption = monthlyConfig.DayOptions switch
        {
            DayOptions.First => _localizer["First"].Value,
            DayOptions.Second => _localizer["Second"].Value,
            DayOptions.Third => _localizer["Third"].Value,
            DayOptions.Fourth => _localizer["Fourth"].Value,
            DayOptions.Last => _localizer["Last"].Value,
            _ => throw new ArgumentOutOfRangeException()
        };

        var weekOption = monthlyConfig.WeekOption switch
        {
            WeekOptions.Weekday => _localizer["Weekday"].Value,
            WeekOptions.WeekendDay => _localizer["WeekendDay"].Value,
            _ => _localizer[monthlyConfig.WeekOption.ToString()].Value
        };

        return string.Format(_localizer["MonthlyDescription_DayOption"].Value, dayOption, weekOption, monthlyConfig.MonthFrequency, dateStr, timeStr, startStr);
    }

    private string GenerateSpecificDayDescription(SpecificDayMonthlySchedulerConfiguration specificDayConfig, DateTime executionTime)
    {
        var dateStr = executionTime.ToString("d", CultureInfo.CurrentCulture);
        var timeStr = executionTime.ToString("t", CultureInfo.CurrentCulture);
        var startStr = specificDayConfig.CurrentDate.ToString("d", CultureInfo.CurrentCulture);

        return string.Format(_localizer["MonthlyDescription_SpecificDay"].Value, specificDayConfig.SpecificDay, specificDayConfig.MonthFrequency, dateStr, timeStr, startStr);
    }

    private string GenerateOnceDescription(OnceSchedulerConfiguration onceConfig, DateTime executionTime)
    {
        var dateStr = executionTime.ToString("d", CultureInfo.CurrentCulture);
        var timeStr = executionTime.ToString("t", CultureInfo.CurrentCulture);
        var startStr = onceConfig.CurrentDate.ToString("d", CultureInfo.CurrentCulture);

        return string.Format(_localizer["OnceDescription"].Value, dateStr, timeStr, startStr);
    }
}
