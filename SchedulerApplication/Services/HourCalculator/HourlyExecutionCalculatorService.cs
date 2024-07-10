using SchedulerApplication.Models;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.HourCalculator;
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
            var currentHour = currentDate.Date.Add(startTime);

            if (startTime <= endTime)
            {
                while (currentHour.TimeOfDay <= endTime && results.Count < 12 && currentHour <= limits.LimitEndDateTime)
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
                while (currentHour.TimeOfDay < TimeSpan.FromHours(24) && results.Count < 12 && currentHour <= limits.LimitEndDateTime)
                {
                    if (currentHour >= limits.LimitStartDateTime && currentHour <= limits.LimitEndDateTime)
                    {
                        results.Add(currentHour);
                    }
                    currentHour = currentHour.AddHours(interval);
                }

                currentHour = currentDate.Date.AddDays(1).Date.Add(startTime); 

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
        }

        return results.Take(12).Where(dt => dt <= limits.LimitEndDateTime);
    }

    private IEnumerable<DateTime> CalculateWeeklyExecutions(WeeklyFrequencyConfiguration config)
    {
        var results = new List<DateTime>();
        var currentDate = config.CurrentDate;
        var startTime = config.HourTimeRange.StartHour;
        var endTime = config.HourTimeRange.EndHour;
        var interval = config.HourTimeRange.HourlyInterval;
        var startLimit = config.Limits.LimitStartDateTime;
        var endLimit = config.Limits.LimitEndDateTime ?? DateTime.MaxValue;
        var weekInterval = config.WeekInterval;
        var daysOfWeek = new HashSet<DayOfWeek>(config.DaysOfWeek); 

        while (currentDate <= endLimit && results.Count < 12)
        {
            if (daysOfWeek.Contains(currentDate.DayOfWeek))
            {
                var currentHour = currentDate.Date.Add(startTime);

                while (currentHour.TimeOfDay <= endTime && results.Count < 12 && currentHour <= endLimit)
                {
                    if (currentHour >= startLimit && currentHour <= endLimit)
                    {
                        results.Add(currentHour);
                    }
                    currentHour = currentHour.AddHours(interval);
                }
            }
            
            currentDate = currentDate.AddDays(1);

            if ((int)currentDate.DayOfWeek == 0) 
            {
                currentDate = currentDate.AddDays(7 * (weekInterval - 1));
            }
        }

        return results.Take(12).Where(dt => dt <= endLimit);
    }
}