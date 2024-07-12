using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.FrequencyConfigurations;

namespace SchedulerApplication.Services.DateCalculatorServices;

public class WeeklyExecutionCalculatorService : IWeeklyExecutionCalculatorService
{
    private readonly IDailyExecutionCalculatorService _dailyExecutionCalculatorService;

    public WeeklyExecutionCalculatorService(IDailyExecutionCalculatorService dailyExecutionCalculatorService)
    {
        _dailyExecutionCalculatorService = dailyExecutionCalculatorService;
    }

    public List<DateTime> CalculateWeeklyExecutions(WeeklyFrequencyConfiguration config, int maxExecutions)
    {
        var executionTimes = new List<DateTime>();
        var weeklyDays = CalculateWeeklyDays(config);

        foreach (var day in weeklyDays)
        {
            var hourlyExecutions = _dailyExecutionCalculatorService.GenerateHourlyExecutionsForDay(config.HourTimeRange, day, config.Limits,config.HourlyInterval);

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

    private static List<DateTime> CalculateWeeklyDays(WeeklyFrequencyConfiguration config)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));
        var results = new List<DateTime>();
        var currentDate = config.CurrentDate;
        var weekInterval = config.WeekInterval;
        var daysOfWeek = new HashSet<DayOfWeek>(config.DaysOfWeek);
        var limitInitDatetime = config.Limits.LimitStartDateTime;
        var endDate = config.Limits.LimitEndDateTime ?? DateTime.MaxValue;

        if (!daysOfWeek.Any())
        {
            return results;
        }

        while (currentDate <= endDate && results.Count < 12)
        {
            if (daysOfWeek.Contains(currentDate.DayOfWeek))
            {
                var executionTime = currentDate.Date;
                if (executionTime >= limitInitDatetime && executionTime <= endDate)
                {
                    results.Add(executionTime);
                }
            }

            currentDate = currentDate.AddDays(1);

            if (currentDate.DayOfWeek == DayOfWeek.Monday && results.Count > 0)
            {
                currentDate = currentDate.AddDays(7 * (weekInterval - 1));
            }
        }

        return results;

    }
}
