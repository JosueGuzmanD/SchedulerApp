using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.SchedulerConfigurations;

public class OnceExecutionCalculator : IOnceExecutionCalculator
{
    public IEnumerable<DateTime> CalculateOnceExecutions(OnceSchedulerConfiguration config)
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
}

