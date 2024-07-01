using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.ExecutionTime;

public class RecurringExecutionService : IRecurringExecutionService
{
    private readonly IConfigurationValidator _validator;
    private readonly IHourCalculatorService _hourCalculator;
    private readonly IWeekCalculatorService _weekCalculator;

    public RecurringExecutionService(IConfigurationValidator validator, IHourCalculatorService hourCalculator, IWeekCalculatorService weekCalculator)
    {
        _validator = validator;
        _hourCalculator = hourCalculator;
        _weekCalculator = weekCalculator;
    }
  
    public List<DateTime> CalculateNextExecutionTimes(RecurringSchedulerConfiguration configuration)
    {
        int maxExecutions = 3;
        int executionCount = 0;

        _validator.Validate(configuration);

        List<DateTime> executionTimes = new List<DateTime>();
        DateTime currentDate = configuration.CurrentDate;
        DateTime endDate = configuration.TimeInterval.LimitEndDateTime ?? DateTime.MaxValue;



        var weeklyDates = _weekCalculator.CalculateWeeklyDates(currentDate, configuration.DaysOfWeek, configuration.WeekInterval);

        foreach (var date in weeklyDates)
        {
            if (executionCount >= maxExecutions) break;

            if (date >= configuration.TimeInterval.LimitStartDateTime && date <= endDate)
            {
                var hourlyExecutions = _hourCalculator.CalculateHour(date, configuration.HourTimeRange);
                executionTimes.AddRange(hourlyExecutions);
                executionCount++;
            }
        }

        return executionTimes;
    }
}

