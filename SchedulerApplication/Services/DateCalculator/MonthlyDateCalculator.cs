using SchedulerApplication.Common.Enums;
using SchedulerApplication.Interfaces;
using SchedulerApplication.Models;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Services.DayOptionStrategies;

namespace SchedulerApplication.Services.DateCalculator;


public class MonthlyDateCalculator : IDateCalculator
{
    private readonly Dictionary<DayOptions, IDateCalculationStrategy> _strategies = new()
    {
        { DayOptions.First, new OrdinalWeekdayStrategy(1) },
        { DayOptions.Second, new OrdinalWeekdayStrategy(2) },
        { DayOptions.Third, new OrdinalWeekdayStrategy(3) },
        { DayOptions.Fourth, new OrdinalWeekdayStrategy(4) },
        { DayOptions.Last, new LastWeekdayStrategy() },
        { DayOptions.SpecificDay, new SpecificDayStrategy() } 
    };

    public List<DateTime> CalculateDates(SchedulerConfiguration config, int maxExecutions)
    {
        if (config is SpecificDayMonthlySchedulerConfiguration specificDayConfig)
        {
            return _strategies[DayOptions.SpecificDay].CalculateDates(specificDayConfig,maxExecutions);
        }

        if (config is not MonthlySchedulerConfiguration monthlyConfig)
        {
            throw new ArgumentException("Invalid configuration type for MonthlyDateCalculator.");
        }

        if (!_strategies.TryGetValue(monthlyConfig.DayOptions, out var strategy))
        {
            throw new ArgumentOutOfRangeException(nameof(monthlyConfig.DayOptions), monthlyConfig.DayOptions, null);
        }

        return strategy.CalculateDates(monthlyConfig, maxExecutions);
    }
}
