using SchedulerApplication.Common.Enums;
using SchedulerApplication.Interfaces;
using SchedulerApplication.Models;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Services.DayOptionStrategies;

namespace SchedulerApplication.Services.DateCalculator;


public class MonthlyDateCalculator : IDateCalculator
{
    private readonly Dictionary<DayOptions, IDateCalculationStrategy> _strategies;

    public MonthlyDateCalculator()
    {
        _strategies = new Dictionary<DayOptions, IDateCalculationStrategy>
        {
            { DayOptions.First, new OrdinalWeekdayStrategy(1) },
            { DayOptions.Second, new OrdinalWeekdayStrategy(2) },
            { DayOptions.Third, new OrdinalWeekdayStrategy(3) },
            { DayOptions.Fourth, new OrdinalWeekdayStrategy(4) },
            { DayOptions.Last, new LastWeekdayStrategy() }
        };
    }

    public List<DateTime> CalculateDates(SchedulerConfiguration config)
    {
        if (config is not MonthlySchedulerConfiguration monthlyConfig)
        {
            throw new ArgumentException("Invalid configuration type for MonthlyDateCalculator.");
        }

        if (!_strategies.ContainsKey(monthlyConfig.DayOptions))
        {
            throw new ArgumentOutOfRangeException(nameof(monthlyConfig.DayOptions), monthlyConfig.DayOptions, null);
        }

        return _strategies[monthlyConfig.DayOptions].CalculateDates(config);
    }
}
