using SchedulerApplication.Common.Enums;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.ExecutionTime;

public class RecurringExecutionService : IRecurringExecutionService
{
    private readonly IConfigurationValidator _validatorService;
    private readonly IHourlyExecutionCalculatorService _hourlyExecutionCalculatorService;
    private readonly IWeeklyExecutionCalculatorService _weeklyExecutionCalculatorService;

    public RecurringExecutionService(
        IConfigurationValidator validatorService,
        IHourlyExecutionCalculatorService hourlyExecutionCalculatorService,
        IWeeklyExecutionCalculatorService weeklyExecutionCalculatorService)
    {
        _validatorService = validatorService;
        _hourlyExecutionCalculatorService = hourlyExecutionCalculatorService;
        _weeklyExecutionCalculatorService = weeklyExecutionCalculatorService;
    }

    public List<DateTime> CalculateNextExecutionTimes(RecurringSchedulerConfiguration configuration)
    {
        const int maxExecutions = 12;
        _validatorService.Validate(configuration);

        return configuration switch
        {
            DailyFrequencyConfiguration dailyConfig => CalculateDailyExecutions(dailyConfig, maxExecutions),
            WeeklyFrequencyConfiguration weeklyConfig => CalculateWeeklyExecutions(weeklyConfig, maxExecutions),
            _ => throw new ArgumentException("Unsupported configuration type.")
        };
    }

    private List<DateTime> CalculateDailyExecutions(DailyFrequencyConfiguration config, int maxExecutions)
    {
        var executionTimes = new List<DateTime>();
        var currentDate = config.CurrentDate;
        var endDate = config.Limits.LimitEndDateTime ?? DateTime.MaxValue;

        while (currentDate <= endDate && executionTimes.Count < maxExecutions)
        {
            if (config.OccursOnce)
            {
                var executionTime = currentDate.Date + config.OnceAt;
                if (config.Limits.IsWithinInterval(executionTime))
                {
                    executionTimes.Add(executionTime);
                }
            }
            else
            {
                var hourlyExecutions = GenerateHourlyExecutionsForDay(config, currentDate);
                foreach (var executionTime in hourlyExecutions)
                {
                    if (executionTimes.Count >= maxExecutions || executionTime > endDate) break;
                    if (config.Limits.IsWithinInterval(executionTime))
                    {
                        executionTimes.Add(executionTime);
                    }
                }
            }

            currentDate = currentDate.AddDays(1);
        }

        return executionTimes;
    }

    private IEnumerable<DateTime> GenerateHourlyExecutionsForDay(DailyFrequencyConfiguration config, DateTime day)
    {
        if (config.HourTimeRange.HourlyFrequency == DailyHourFrequency.Once)
        {
            return new List<DateTime> { day.Date.Add(config.HourTimeRange.StartHour) };
        }
        else
        {
            var results = new List<DateTime>();
            var currentHour = day.Date.Add(config.HourTimeRange.StartHour);
            var endHour = day.Date.Add(config.HourTimeRange.EndHour);
            var interval = config.HourTimeRange.HourlyInterval;

            while (currentHour <= endHour && results.Count < 12)
            {
                if (currentHour >= config.Limits.LimitStartDateTime && currentHour <= config.Limits.LimitEndDateTime)
                {
                    results.Add(currentHour);
                }
                currentHour = currentHour.AddHours(interval);
            }

            return results;
        }
    }

    private List<DateTime> CalculateWeeklyExecutions(WeeklyFrequencyConfiguration config, int maxExecutions)
    {
        var executionTimes = new List<DateTime>();
        var weeklyDays = _weeklyExecutionCalculatorService.CalculateWeeklyExecutions(config);

        foreach (var day in weeklyDays)
        {
            var hourlyExecutions = GenerateHourlyExecutionsForDay(config, day);

            foreach (var executionTime in hourlyExecutions)
            {
                if (executionTimes.Count >= maxExecutions)
                {
                    return executionTimes;
                }
                executionTimes.Add(executionTime);
            }
        }

        return executionTimes;
    }

    private IEnumerable<DateTime> GenerateHourlyExecutionsForDay(WeeklyFrequencyConfiguration config, DateTime day)
    {
        if (config.HourTimeRange.HourlyFrequency == DailyHourFrequency.Once)
        {
            return new List<DateTime> { day.Date.Add(config.HourTimeRange.StartHour) };
        }
        else
        {
            var results = new List<DateTime>();
            var currentHour = day.Date.Add(config.HourTimeRange.StartHour);
            var endHour = day.Date.Add(config.HourTimeRange.EndHour);
            var interval = config.HourTimeRange.HourlyInterval;

            while (currentHour <= endHour && results.Count < 12)
            {
                if (currentHour >= config.Limits.LimitStartDateTime && currentHour <= config.Limits.LimitEndDateTime)
                {
                    results.Add(currentHour);
                }
                currentHour = currentHour.AddHours(interval);
            }

            return results;
        }
    }
}
