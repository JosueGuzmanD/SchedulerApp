using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.ExecutionTime;

public class RecurringExecutionService : IRecurringExecutionService
{
    private readonly IConfigurationValidator _validator;
    private readonly IHourCalculatorService _hourCalculator;
    private readonly IWeekCalculatorService _weekCalculator;

    public RecurringExecutionService(IConfigurationValidator validator, IHourCalculatorService hourCalculator,
        IWeekCalculatorService weekCalculator)
    {
        _validator = validator;
        _hourCalculator = hourCalculator;
        _weekCalculator = weekCalculator;
    }

    public List<DateTime> CalculateNextExecutionTime(RecurringSchedulerConfiguration configuration)
    {
        const int maxExecutions = 12;

        _validator.Validate(configuration);

        switch (configuration)
        {
            case DailyFrequencyConfiguration dailyConfig:
                return CalculateDailyExecutions(dailyConfig, maxExecutions);
            case WeeklyFrequencyConfiguration weeklyConfig:
                return CalculateWeeklyExecutions(weeklyConfig, maxExecutions);
            default:
                throw new ArgumentException("Unsupported configuration type.");
        }
    }

    private List<DateTime> CalculateDailyExecutions(DailyFrequencyConfiguration config, int maxExecutions)
    {
        var executionTimes = new List<DateTime>();
        var currentDate = config.CurrentDate;
        var endDate = config.TimeInterval.LimitEndDateTime ?? DateTime.MaxValue;
        var executionCount = 0;

        for (var date = currentDate; date <= endDate; date = date.AddDays(config.DaysInterval))
        {
            if (executionCount >= maxExecutions) break;
            if (!config.TimeInterval.IsWithinInterval(date)) continue;

            var hourlyExecutions = _hourCalculator.CalculateHour(date, config.HourTimeRange);
            AddExecutions(hourlyExecutions, executionTimes, ref executionCount, maxExecutions);
        }

        return executionTimes;
    }

    private List<DateTime> CalculateWeeklyExecutions(WeeklyFrequencyConfiguration config, int maxExecutions)
    {
        var executionTimes = new List<DateTime>();
        var currentDate = config.CurrentDate;
        var endDate = config.TimeInterval.LimitEndDateTime ?? DateTime.MaxValue;
        var executionCount = 0;

        var weeklyDates = _weekCalculator.CalculateWeeklyDates(currentDate, config.DaysOfWeek, config.WeekInterval);
        foreach (var date in weeklyDates)
        {
            if (executionCount >= maxExecutions) break;
            if (!config.TimeInterval.IsWithinInterval(date)) continue;

            var hourlyExecutions = _hourCalculator.CalculateHour(date, config.HourTimeRange);
            AddExecutions(hourlyExecutions, executionTimes, ref executionCount, maxExecutions);
        }

        return executionTimes;
    }

    private void AddExecutions(IEnumerable<DateTime> hourlyExecutions, List<DateTime> executionTimes, ref int executionCount, int maxExecutions)
    {
        foreach (var executionTime in hourlyExecutions)
        {
            if (executionCount >= maxExecutions) break;
            executionTimes.Add(executionTime);
            executionCount++;
        }
    }
}