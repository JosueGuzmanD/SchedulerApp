using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Models.ValueObjects;
using SchedulerApplication.Models;
using SchedulerApplication.Services.HourCalculator;


namespace SchedulerApplication.Services.ExecutionTime;

public class ExecutionTimeGenerator : IExecutionTimeGenerator
{
    private readonly IDateCalculator _onceDateCalculator;
    private readonly IDateCalculator _dailyDateCalculator;
    private readonly IDateCalculator _weeklyDateCalculator;
    private readonly HourCalculatorService _hourCalculator;

    public ExecutionTimeGenerator(
        IDateCalculator onceDateCalculator,
        IDateCalculator dailyDateCalculator,
        IDateCalculator weeklyDateCalculator,
        HourCalculatorService hourCalculator)
    {
        _onceDateCalculator = onceDateCalculator;
        _dailyDateCalculator = dailyDateCalculator;
        _weeklyDateCalculator = weeklyDateCalculator;
        _hourCalculator = hourCalculator;
    }

    public List<DateTime> GenerateExecutions(SchedulerConfiguration configuration)
    {
        if (!configuration.IsEnabled)
        {
            return new List<DateTime>();
        }

        ValidateConfiguration(configuration);


        List<DateTime> dates;

        switch (configuration)
        {
            case OnceSchedulerConfiguration onceConfig:
                dates = _onceDateCalculator.CalculateDates(onceConfig);
                return dates;
            case DailyFrequencyConfiguration dailyConfig:
                dates = _dailyDateCalculator.CalculateDates(dailyConfig);
                break;
            case WeeklyFrequencyConfiguration weeklyConfig:
                dates = _weeklyDateCalculator.CalculateDates(weeklyConfig);
                break;
            default:
                throw new ArgumentException("Unknown configuration type");
        }

        var hourTimeRange = (configuration as RecurringSchedulerConfiguration)?.HourTimeRange ?? new HourTimeRange(new TimeSpan(0, 0, 0));
        var hourlyInterval = (configuration as RecurringSchedulerConfiguration)?.HourlyInterval ?? 0;

        var results = _hourCalculator.CalculateHours(dates, hourTimeRange, hourlyInterval);

        if (configuration.Limits != null)
        {
            var endLimit = configuration.Limits.LimitEndDateTime?.Date.AddDays(1) ?? DateTime.MaxValue;
            results = results.Where(result => result >= configuration.Limits.LimitStartDateTime && result < endLimit).ToList();
        }

        return results.Take(12).ToList();
    }

    private void ValidateConfiguration(SchedulerConfiguration configuration)
    {
        if (configuration is RecurringSchedulerConfiguration { HourlyInterval: < 0 })
        {
            throw new ArgumentException("Invalid hourly interval");
        }

        if (configuration.Limits.LimitEndDateTime < configuration.Limits.LimitStartDateTime)
        {
            throw new ArgumentException("End date must be greater than or equal to start date");
        }
    }
}
