﻿using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Models.ValueObjects;
using SchedulerApplication.Models;
using SchedulerApplication.Services.DateCalculator;
using SchedulerApplication.Services.HourCalculator;

namespace SchedulerApplication.Services.ExecutionTime;

public class ExecutionTimeGenerator : IExecutionTimeGenerator
{
    private readonly IDateCalculator _onceDateCalculator;
    private readonly IDateCalculator _dailyDateCalculator;
    private readonly IDateCalculator _weeklyDateCalculator;
    private readonly IHourCalculator _hourCalculator;

    public ExecutionTimeGenerator()
    {
        _onceDateCalculator = new OnceDateCalculator();
        _dailyDateCalculator = new DailyDateCalculator();
        _weeklyDateCalculator = new WeeklyDateCalculator();
        _hourCalculator = new HourCalculatorService();
    }

    public List<DateTime> GenerateExecutions(SchedulerConfiguration configuration)
    {
        if (!configuration.IsEnabled)
        {
            return new List<DateTime>();
        }

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

        return _hourCalculator.CalculateHours(dates, hourTimeRange, hourlyInterval);
    }
}
