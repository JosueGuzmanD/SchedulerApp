using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.ExecutionTime;

public class RecurringExecutionService : IRecurringExecutionService
{
    private readonly IConfigurationValidator _validatorService;
    private readonly IDailyExecutionCalculatorService _dailyExecutionCalculatorService;
    private readonly IWeeklyExecutionCalculatorService _weeklyExecutionCalculatorService;

    public RecurringExecutionService(
        IConfigurationValidator validatorService,
        IDailyExecutionCalculatorService dailyExecutionCalculatorService,
        IWeeklyExecutionCalculatorService weeklyExecutionCalculatorService)
    {
        _validatorService = validatorService;
        _dailyExecutionCalculatorService = dailyExecutionCalculatorService;
        _weeklyExecutionCalculatorService = weeklyExecutionCalculatorService;
    }

    public List<DateTime> CalculateNextExecutionTimes(RecurringSchedulerConfiguration configuration)
    {
        const int maxExecutions = 12;
        _validatorService.Validate(configuration);

        return configuration switch
        {
            DailyFrequencyConfiguration dailyConfig => _dailyExecutionCalculatorService.CalculateDailyExecutions(dailyConfig, maxExecutions),
            WeeklyFrequencyConfiguration weeklyConfig => _weeklyExecutionCalculatorService.CalculateWeeklyExecutions(weeklyConfig, maxExecutions),
            _ => throw new ArgumentException("Unsupported configuration type.")
        };
    }
}