using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.WeekCalculator;

public class WeeklyExecutionCalculatorService : IWeeklyExecutionCalculatorService
{
    private readonly IHourlyExecutionCalculatorService _hourlyExecutionCalculatorService;

    public WeeklyExecutionCalculatorService(IHourlyExecutionCalculatorService hourlyExecutionCalculatorService)
    {
        _hourlyExecutionCalculatorService = hourlyExecutionCalculatorService;
    }

    public List<DateTime> CalculateWeeklyExecutions(WeeklyFrequencyConfiguration config, int maxExecutions)
    {
        var executionTimes = new List<DateTime>();
        var currentDate = config.CurrentDate;
        var executionCount = 0;

        while (executionCount < maxExecutions)
        {
            foreach (var dayOfWeek in config.DaysOfWeek)
            {
                var nextValidDate = GetNextValidDate(currentDate, dayOfWeek);
                if (nextValidDate >= config.CurrentDate && executionCount < maxExecutions)
                {
                    var hourlyExecutions = _hourlyExecutionCalculatorService.CalculateHourlyExecutions(
                        new WeeklyFrequencyConfiguration()
                        {
                            IsEnabled = config.IsEnabled,
                            CurrentDate = nextValidDate,
                            HourTimeRange = config.HourTimeRange
                        }
                    );
                        
                    foreach (var exec in hourlyExecutions)
                    {
                        if (executionCount < maxExecutions)
                        {
                            executionTimes.Add(exec);
                            executionCount++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Increment by the week interval
            currentDate = currentDate.AddDays(7 * config.WeekInterval);
        }

        return executionTimes;
    }

    private DateTime GetNextValidDate(DateTime startDate, DayOfWeek targetDay)
    {
        var daysUntilNextTargetDay = ((int)targetDay - (int)startDate.DayOfWeek + 7) % 7;
        return startDate.AddDays(daysUntilNextTargetDay);
    }
}

