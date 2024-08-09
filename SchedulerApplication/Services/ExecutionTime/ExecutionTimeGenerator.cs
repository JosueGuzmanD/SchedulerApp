using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Models.ValueObjects;
using SchedulerApplication.Models;
using SchedulerApplication.Services.HourCalculator;
using SchedulerApplication.Common.Enums;


namespace SchedulerApplication.Services.ExecutionTime;

public class ExecutionTimeGenerator : IExecutionTimeGenerator
{
    private readonly IDateCalculator _onceDateCalculator;
    private readonly IDateCalculator _dailyDateCalculator;
    private readonly IDateCalculator _weeklyDateCalculator;
    private readonly HourCalculatorService _hourCalculator;
    private readonly IDateCalculator _monthlyDateCalculator;
    private readonly CustomStringLocalizer _customStringLocalizer;

    public ExecutionTimeGenerator(
        IDateCalculator onceDateCalculator,
        IDateCalculator dailyDateCalculator,
        IDateCalculator weeklyDateCalculator,
        HourCalculatorService hourCalculator,
        IDateCalculator monthlyDateCalculator,
        CustomStringLocalizer customStringLocalizer)
    {
        _onceDateCalculator = onceDateCalculator;
        _dailyDateCalculator = dailyDateCalculator;
        _weeklyDateCalculator = weeklyDateCalculator;
        _monthlyDateCalculator = monthlyDateCalculator;
        _hourCalculator = hourCalculator;
        _customStringLocalizer= customStringLocalizer;
    }

    public List<DateTime> GenerateExecutions(SchedulerConfiguration configuration, int maxExecutions)
    {
        if (!configuration.IsEnabled)
        {
            return [];
        }

        ValidateConfiguration(configuration);

        List<DateTime> dates;

        switch (configuration)
        {
            case OnceSchedulerConfiguration onceConfig:
                dates = _onceDateCalculator.CalculateDates(onceConfig, maxExecutions);
                return dates;
            case DailyFrequencyConfiguration dailyConfig:
                dates = _dailyDateCalculator.CalculateDates(dailyConfig, maxExecutions);
                break;
            case WeeklyFrequencyConfiguration weeklyConfig:
                dates = _weeklyDateCalculator.CalculateDates(weeklyConfig, maxExecutions);
                break;
            case MonthlySchedulerConfiguration monthlyConfig:
                dates = _monthlyDateCalculator.CalculateDates(monthlyConfig, maxExecutions);
                break;
            default:
                throw new ArgumentException("Unknown configuration type");
        }

        var recurringConfig = configuration as RecurringSchedulerConfiguration;
        var hourTimeRange = recurringConfig?.HourTimeRange ?? new HourTimeRange(new TimeSpan(0, 0, 0));
        var interval = recurringConfig?.Interval ?? 0;
        var intervalType = recurringConfig?.IntervalType ?? IntervalType.Hourly;

        var results = _hourCalculator.CalculateHours(dates, hourTimeRange, interval, intervalType,configuration.Limits, maxExecutions);

        if (configuration.Limits == null) return results.Take(maxExecutions).ToList();
        var endLimit = configuration.Limits.LimitEndDateTime?.Date.AddDays(1) ?? DateTime.MaxValue;
        results = results.Where(result => result >= configuration.Limits.LimitStartDateTime && result < endLimit).ToList();

        return results.Take(maxExecutions).ToList();
    }

    private void ValidateConfiguration(SchedulerConfiguration configuration)
    {
        if (configuration is RecurringSchedulerConfiguration { Interval: < 0 })
        {
            var localizedMessage = _customStringLocalizer["ExecutionTimeGeneratorInvalidIntervalExc", configuration.Culture].Value;
            throw new ArgumentException(localizedMessage);
        }
    }
}
