using SchedulerApplication.Models;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;
using SchedulerApplication.ValueObjects;

namespace SchedulerApplication.Services.HourCalculator;

/*public class HourlyExecutionCalculatorService : IHourlyExecutionCalculatorService
{
    public List<DateTime> CalculateHourlyExecutions(SchedulerConfiguration config)
    {
        return config switch
        {
            OnceSchedulerConfiguration onceConfig => CalculateOnceExecutions(onceConfig),
            DailyFrequencyConfiguration dailyConfig => CalculateDailyExecutions(dailyConfig),
            WeeklyFrequencyConfiguration weeklyConfig => CalculateWeeklyExecutions(weeklyConfig),
            _ => throw new ArgumentException("Unsupported configuration type.")
        };
    }

    private List<DateTime> CalculateOnceExecutions(OnceSchedulerConfiguration config)
    {
        var executions = new List<DateTime>();
        var currentTime = config.CurrentDate.Date + config.ConfigurationDateTime.TimeOfDay;

        while (executions.Count < 12)
        {
            executions.Add(currentTime);
            currentTime = currentTime.AddDays(1);
        }

        return executions;
    }

    private List<DateTime> CalculateDailyExecutions(DailyFrequencyConfiguration config)
    {
        var executions = new List<DateTime>();
        var currentTime = config.CurrentDate.Date + config.HourTimeRange.StartHour;
        var endDateTime = config.Limits.LimitEndDateTime;

        while (executions.Count < 12 && currentTime <= endDateTime)
        {
            if (config.OccursOnce)
            {
                var executionTime = config.CurrentDate.Date + config.OnceAt;
                if (executionTime <= endDateTime)
                {
                    executions.Add(executionTime);
                }
                return executions;
            }

            if (ShouldAddExecutionTime(currentTime, config.HourTimeRange))
            {
                executions.Add(currentTime);
            }

            currentTime = currentTime.AddHours(config.HourTimeRange.HourlyInterval);

            if (ShouldMoveToNextDay(currentTime, config.HourTimeRange))
            {
                currentTime = MoveToNextDay(currentTime, config.HourTimeRange);
            }
        }

        return executions;
    }

    private List<DateTime> CalculateWeeklyExecutions(WeeklyFrequencyConfiguration config)
    {
        var executions = new List<DateTime>();
        var currentDate = config.CurrentDate;
        var endDateTime = config.Limits.LimitEndDateTime;

        while (executions.Count < 12 && currentDate <= endDateTime)
        {
            foreach (var dayOfWeek in config.DaysOfWeek)
            {
                var nextValidDate = GetNextValidDate(currentDate, dayOfWeek, config.WeekInterval);
                if (nextValidDate > endDateTime) break;

                var dailyExecutions = CalculateDailyExecutions(new DailyFrequencyConfiguration
                {
                    IsEnabled = config.IsEnabled,
                    CurrentDate = nextValidDate,
                    HourTimeRange = config.HourTimeRange,
                    Limits = config.Limits
                });

                foreach (var exec in dailyExecutions)
                {
                    if (executions.Count < 12)
                    {
                        executions.Add(exec);
                    }
                    else
                    {
                        return executions;
                    }
                }
            }

            currentDate = currentDate.AddDays(7 * config.WeekInterval);
        }

        return executions;
    }

    private bool ShouldAddExecutionTime(DateTime currentTime, HourTimeRange hourTimeRange)
    {
        return currentTime.TimeOfDay >= hourTimeRange.StartHour && currentTime.TimeOfDay <= hourTimeRange.EndHour;
    }

    private bool ShouldMoveToNextDay(DateTime currentTime, HourTimeRange hourTimeRange)
    {
        return currentTime.TimeOfDay > hourTimeRange.EndHour ||
               (hourTimeRange.EndHour < hourTimeRange.StartHour && currentTime.TimeOfDay < hourTimeRange.StartHour);
    }

    private DateTime MoveToNextDay(DateTime currentTime, HourTimeRange hourTimeRange)
    {
        return currentTime.Date.AddDays(1) + hourTimeRange.StartHour;
    }

    private DateTime GetNextValidDate(DateTime startDate, DayOfWeek targetDay, int weekInterval)
    {
        var daysUntilNextTargetDay = ((int)targetDay - (int)startDate.DayOfWeek + 7) % 7;
        return startDate.AddDays(daysUntilNextTargetDay).AddDays(7 * (weekInterval - 1));
    }
}*/
public class HourlyExecutionCalculatorService : IHourlyExecutionCalculatorService
{
    public IEnumerable<DateTime> CalculateHourlyExecutions(SchedulerConfiguration config)
    {
        if (!config.IsEnabled)
        {
            return Enumerable.Empty<DateTime>();
        }

        switch (config)
        {
            case OnceSchedulerConfiguration onceConfig:
                return CalculateOnceExecutions(onceConfig);

            case DailyFrequencyConfiguration dailyConfig:
                return CalculateDailyExecutions(dailyConfig);

            case WeeklyFrequencyConfiguration weeklyConfig:
                return CalculateWeeklyExecutions(weeklyConfig);

            default:
                throw new ArgumentException("Unknown configuration type");
        }
    }

    private IEnumerable<DateTime> CalculateOnceExecutions(OnceSchedulerConfiguration config)
    {
        var results = new List<DateTime>();
        var currentDate = config.CurrentDate.Date;
        var configurationDateTime = config.ConfigurationDateTime.TimeOfDay;

        for (int i = 0; i < 12; i++)
        {
            results.Add(currentDate.Add(configurationDateTime));
            currentDate = currentDate.AddDays(1);
        }

        return results;
    }

    private IEnumerable<DateTime> CalculateDailyExecutions(DailyFrequencyConfiguration config)
    {
        var results = new List<DateTime>();
        var currentDate = config.CurrentDate;
        var startTime = config.HourTimeRange.StartHour;
        var endTime = config.HourTimeRange.EndHour;
        var interval = config.HourTimeRange.HourlyInterval;
        var limits = config.Limits;

        while (currentDate <= limits.LimitEndDateTime && results.Count < 12)
        {
            if (startTime <= endTime)
            {
                var currentHour = currentDate.Date.Add(startTime);
                while (currentHour.TimeOfDay <= endTime && results.Count < 12)
                {
                    if (currentHour >= limits.LimitStartDateTime && currentHour <= limits.LimitEndDateTime)
                    {
                        results.Add(currentHour);
                    }
                    currentHour = currentHour.AddHours(interval);
                }
            }
            else
            {
                var currentHour = currentDate.Date.Add(startTime);
                while (currentHour.TimeOfDay < TimeSpan.FromHours(24) && results.Count < 12 && currentHour <= limits.LimitEndDateTime)
                {
                    if (currentHour >= limits.LimitStartDateTime && currentHour <= limits.LimitEndDateTime)
                    {
                        results.Add(currentHour);
                    }
                    currentHour = currentHour.AddHours(interval);
                }

                currentHour = currentDate.Date.AddDays(1); 
                while (currentHour.TimeOfDay <= endTime && results.Count < 12)
                {
                    if (currentHour >= limits.LimitStartDateTime && currentHour <= limits.LimitEndDateTime)
                    {
                        results.Add(currentHour);
                    }
                    currentHour = currentHour.AddHours(interval);
                }
            }

            currentDate = currentDate.AddDays(1);
        }

        return results;
    }

    private IEnumerable<DateTime> CalculateWeeklyExecutions(WeeklyFrequencyConfiguration config)
    {
        var results = new List<DateTime>();
        var currentDate = config.CurrentDate;
        var startTime = config.HourTimeRange.StartHour;
        var endTime = config.HourTimeRange.EndHour;
        var interval = config.HourTimeRange.HourlyInterval;
        var limits = config.Limits;
        var weekInterval = config.WeekInterval;
        var daysOfWeek = new HashSet<DayOfWeek>(config.DaysOfWeek); 

        while (currentDate <= limits.LimitEndDateTime && results.Count < 12)
        {
            if (daysOfWeek.Contains(currentDate.DayOfWeek))
            {
                var currentHour = currentDate.Date.Add(startTime);

                while (currentHour.TimeOfDay <= endTime && results.Count < 12 && currentHour <= limits.LimitEndDateTime)
                {
                    if (currentHour >= limits.LimitStartDateTime && currentHour <= limits.LimitEndDateTime)
                    {
                        results.Add(currentHour);
                    }
                    currentHour = currentHour.AddHours(interval);
                }
            }

            currentDate = currentDate.AddDays(1);

            if ((int)currentDate.DayOfWeek == 0) // If it's Sunday, check week interval
            {
                currentDate = currentDate.AddDays(7 * (weekInterval - 1));
            }
        }

        // Ensure to limit the results to the limit end date time
        return results.Take(12).Where(dt => dt <= limits.LimitEndDateTime);
    }
}