using SchedulerApplication.Common.Enums;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.HourCalculator;

public class HourlyExecutionCalculatorService : IHourlyExecutionCalculatorService
{
    public List<DateTime> CalculateHourlyExecutions(RecurringSchedulerConfiguration config)
    {
        var executions = new List<DateTime>();
        var currentTime = config.CurrentDate.Date + config.HourTimeRange.StartHour;

        while (executions.Count < 12)
        {
            executions.Add(currentTime);

            currentTime = currentTime.AddHours(config.HourTimeRange.HourlyInterval);

            if (currentTime.TimeOfDay > config.HourTimeRange.EndHour)
            {
                if (config.HourTimeRange.EndHour < config.HourTimeRange.StartHour)
                {
                    if (currentTime.TimeOfDay <= DateTime.MaxValue.TimeOfDay)
                    {
                        currentTime = currentTime.Date.AddDays(1) + config.HourTimeRange.StartHour;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            if (currentTime.TimeOfDay > config.HourTimeRange.EndHour && currentTime.TimeOfDay <= DateTime.MaxValue.TimeOfDay)
            {
                break;
            }
        }

        return executions;
    }
    /*
    public List<DateTime> CalculateHourlyExecutions(OnceSchedulerConfiguration config)
    {
        return new List<DateTime> { config.CurrentDate.Date + config.ConfigurationDateTime.Hour };
    }
    */
}
