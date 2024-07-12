using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Models;

namespace SchedulerApplication.Services.HourCalculatorServices;

public class ExecutionTimeGenerator : IExecutionTimeGenerator
{
    private readonly IOnceExecutionCalculator _onceExecutionCalculator;
    private readonly IDailyExecutionCalculator _dailyExecutionCalculator;
    private readonly IWeeklyExecutionCalculator _weeklyExecutionCalculator;

    public ExecutionTimeGenerator(
        IOnceExecutionCalculator onceExecutionCalculator,
        IDailyExecutionCalculator dailyExecutionCalculator,
        IWeeklyExecutionCalculator weeklyExecutionCalculator)
    {
        _onceExecutionCalculator = onceExecutionCalculator;
        _dailyExecutionCalculator = dailyExecutionCalculator;
        _weeklyExecutionCalculator = weeklyExecutionCalculator;
    }

    public IEnumerable<DateTime> GenerateExecutions(SchedulerConfiguration config)
    {
        if (!config.IsEnabled)
        {
            return Enumerable.Empty<DateTime>();
        }

        return config switch
        {
            OnceSchedulerConfiguration onceConfig => _onceExecutionCalculator.CalculateOnceExecutions(onceConfig),
            DailyFrequencyConfiguration dailyConfig => _dailyExecutionCalculator.CalculateDailyExecutions(dailyConfig),
            WeeklyFrequencyConfiguration weeklyConfig => _weeklyExecutionCalculator.CalculateWeeklyExecutions(weeklyConfig),
            _ => throw new ArgumentException("Unknown configuration type")
        };
    }
}

