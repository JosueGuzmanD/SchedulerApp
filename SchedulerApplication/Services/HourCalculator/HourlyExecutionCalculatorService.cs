using SchedulerApplication.Common.Enums;
using SchedulerApplication.Models;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;
using SchedulerApplication.ValueObjects;

namespace SchedulerApplication.Services.HourCalculator;

public class HourlyExecutionCalculatorService : IHourlyExecutionCalculatorService
{
    public List<DateTime> CalculateHourlyExecutions(SchedulerConfiguration config)
    {
        return config switch
        {
            OnceSchedulerConfiguration onceConfig => CalculateOnceExecutions(onceConfig),
            RecurringSchedulerConfiguration recurringConfig => CalculateRecurringExecutions(recurringConfig),
            _ => throw new ArgumentException("Unsupported configuration type.")
        };
    }

    private List<DateTime> CalculateOnceExecutions(OnceSchedulerConfiguration config)
    {
        var executions = new List<DateTime>();
        var currentTime = config.CurrentDate.Date + config.ConfigurationDateTime.TimeOfDay;

        while (executions.Count < 12 && currentTime <= config.ConfigurationDateTime)
        {
            executions.Add(currentTime);
            currentTime = currentTime.AddDays(1);
        }

        return executions;
    }

    private List<DateTime> CalculateRecurringExecutions(RecurringSchedulerConfiguration config)
    {
        var executions = new List<DateTime>();
        var currentDate = config.CurrentDate;
        var executionCount = 0;

        while (executionCount < 12)
        {
            if (config is WeeklyFrequencyConfiguration weeklyConfig)
            {
                foreach (var dayOfWeek in weeklyConfig.DaysOfWeek)
                {
                    var nextValidDate = GetNextValidDate(currentDate, dayOfWeek, weeklyConfig.WeekInterval);
                    var dailyExecutions = CalculateDailyExecutions(nextValidDate, weeklyConfig.HourTimeRange);

                    foreach (var exec in dailyExecutions)
                    {
                        if (executionCount < 12)
                        {
                            executions.Add(exec);
                            executionCount++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                currentDate = currentDate.AddDays(7 * weeklyConfig.WeekInterval);
            }
            else if (config is DailyFrequencyConfiguration dailyConfig)
            {
                var dailyExecutions = CalculateDailyExecutions(currentDate, dailyConfig.HourTimeRange);

                foreach (var exec in dailyExecutions)
                {
                    if (executionCount < 12)
                    {
                        executions.Add(exec);
                        executionCount++;
                    }
                    else
                    {
                        break;
                    }
                }

                currentDate = currentDate.AddDays(1);
            }
        }

        return executions;
    }

    private List<DateTime> CalculateDailyExecutions(DateTime currentDate, HourTimeRange hourTimeRange)
    {
        var executions = new List<DateTime>();
        var currentTime = currentDate.Date + hourTimeRange.StartHour;

        while (true) 
        {
            if (ShouldAddExecutionTime(currentTime, hourTimeRange))
            {
                executions.Add(currentTime);
            }

            currentTime = currentTime.AddHours(hourTimeRange.HourlyInterval);

            if (ShouldMoveToNextDay(currentTime, hourTimeRange))
            {
                currentTime = MoveToNextDay(currentTime, hourTimeRange);
                if (executions.Count >= 12) break;
            }
            else
            {
                break;
            }
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
               (hourTimeRange.EndHour < hourTimeRange.StartHour && currentTime.TimeOfDay <= hourTimeRange.StartHour);
    }

    private DateTime MoveToNextDay(DateTime currentTime, HourTimeRange hourTimeRange)
    {
        return currentTime.Date.AddDays(1) + hourTimeRange.StartHour;
    }

    private DateTime GetNextValidDate(DateTime startDate, DayOfWeek targetDay, int weekInterval)
    {
        var daysUntilNextTargetDay = ((int)targetDay - (int)startDate.DayOfWeek + 7) % 7;
        return startDate.AddDays(daysUntilNextTargetDay).AddDays(7 * weekInterval);
    }
}