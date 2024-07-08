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

        if (configuration is DailyFrequencyConfiguration dailyConfig)
        {
            return CalculateDailyExecutions(dailyConfig, maxExecutions);
        }
        else if (configuration is WeeklyFrequencyConfiguration weeklyConfig)
        {
            return _weeklyExecutionCalculatorService.CalculateWeeklyExecutions(weeklyConfig, maxExecutions);
        }
        else
        {
            throw new ArgumentException("Unsupported configuration type.");
        }
    }
    
    private List<DateTime> CalculateDailyExecutions(DailyFrequencyConfiguration config, int maxExecutions)
    {
        var executionTimes = new List<DateTime>();
        var currentDate = config.CurrentDate;
        var endDate = config.Limits.LimitEndDateTime ?? DateTime.MaxValue;
        var executionCount = 0;

        for (var date = currentDate; date <= endDate; date = date.AddDays(1))
        {
            if (executionCount >= maxExecutions) break;
            if (!config.Limits.IsWithinInterval(date)) continue;

            if (config.OccursOnce)
            {
                var executionTime = date.Date + config.OnceAt;
                if (config.Limits.IsWithinInterval(executionTime))
                {
                    executionTimes.Add(executionTime);
                    executionCount++;
                }
            }
            else
            {
                var hourlyExecutions = _hourlyExecutionCalculatorService.CalculateHourlyExecutions(config);
                AddExecutions(hourlyExecutions, executionTimes, ref executionCount, maxExecutions);
            }
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