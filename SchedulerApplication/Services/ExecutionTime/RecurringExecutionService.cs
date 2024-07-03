using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.ExecutionTime;

public class RecurringExecutionService : IRecurringExecutionService
{
    private readonly IConfigurationValidator _validator;
    private readonly IHourCalculatorService _hourCalculator;
    private readonly IWeekCalculatorService _weekCalculator;

    public RecurringExecutionService(IConfigurationValidator validator, IHourCalculatorService hourCalculator,
        IWeekCalculatorService weekCalculator)
    {
        _validator = validator;
        _hourCalculator = hourCalculator;
        _weekCalculator = weekCalculator;
    }

    public List<DateTime> CalculateNextExecutionTime(RecurringSchedulerConfiguration configuration)
    {
        const int maxExecutions = 3;
        var executionCount = 0;

        _validator.Validate(configuration);

        var executionTimes = new List<DateTime>();
        var currentDate = configuration.CurrentDate;
        var endDate = configuration.TimeInterval.LimitEndDateTime ?? DateTime.MaxValue;


        var weeklyDates =
            _weekCalculator.CalculateWeeklyDates(currentDate, configuration.DaysOfWeek, configuration.WeekInterval);

        foreach (var date in weeklyDates)
        {
            if (executionCount >= maxExecutions) break;

            if (date < configuration.TimeInterval.LimitStartDateTime || date > endDate) continue;
            var hourlyExecutions = _hourCalculator.CalculateHour(date, configuration.HourTimeRange);
            executionTimes.AddRange(hourlyExecutions);
            executionCount++;
        }

        return executionTimes;
    }
}

