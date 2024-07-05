using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Common.Validator;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.ExecutionTime;
using SchedulerApplication.Services.HourCalculator;
using SchedulerApplication.Services.Interfaces;
using SchedulerApplication.Services.WeekCalculator;
using SchedulerApplication.ValueObjects;

public class RecurringExecutionServiceTests
{
    private readonly IConfigurationValidator _validator;
    private readonly IHourCalculatorService _hourCalculator;
    private readonly IWeekCalculatorService _weekCalculator;
    private readonly IRecurringExecutionService _recurringExecutionService;

    public RecurringExecutionServiceTests()
    {
        _validator = new ConfigurationValidator();
        _hourCalculator = new HourCalculatorService();
        _weekCalculator = new WeekCalculatorService();
        _recurringExecutionService = new RecurringExecutionService(_validator, _hourCalculator, _weekCalculator);
    }

    [Fact]
    public void CalculateNextExecutionTime_ShouldThrowException_WhenConfigurationIsDisabled()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            IsEnabled = false,
            CurrentDate = DateTime.Now,
            TimeInterval = new LimitsTimeInterval(DateTime.Now)
        };

        // Act
        Action act = () => _recurringExecutionService.CalculateNextExecutionTime(configuration);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Configuration must be enabled.");
    }

    [Fact]
    public void CalculateNextExecutionTime_ShouldThrowException_WhenUnsupportedConfigurationType()
    {
        // Arrange
        var configuration = new RecurringSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = DateTime.Now,
            TimeInterval = new LimitsTimeInterval(DateTime.Now)
        };

        // Act
        Action act = () => _recurringExecutionService.CalculateNextExecutionTime(configuration);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Unsupported configuration type.");
    }

    [Fact]
    public void CalculateNextExecutionTime_ShouldReturnCorrectExecutions_ForDailyFrequency()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 1, 1),
            TimeInterval = new LimitsTimeInterval(new DateTime(2024, 1, 1), new DateTime(2024, 1, 10)),
            DaysInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        var result = _recurringExecutionService.CalculateNextExecutionTime(configuration);

        // Assert
        result.Should().HaveCount(12); // Limited to 12 executions in total
    }

    [Fact]
    public void CalculateNextExecutionTime_ShouldReturnCorrectExecutions_ForWeeklyFrequency()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 1, 1),
            TimeInterval = new LimitsTimeInterval(new DateTime(2024, 1, 1), new DateTime(2024, 2, 1)),
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
            WeekInterval = 1,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        var result = _recurringExecutionService.CalculateNextExecutionTime(configuration);

        // Assert
        result.Should().HaveCount(12); 
    }

    [Theory]
    [InlineData("2024-01-01", "2024-01-10", 1, 12)] 
    [InlineData("2024-01-01", "2024-01-20", 2, 12)]
    public void CalculateNextExecutionTime_ShouldLimitExecutions_ForDailyFrequency(string start, string end, int daysInterval, int expectedCount)
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = DateTime.Parse(start),
            TimeInterval = new LimitsTimeInterval(DateTime.Parse(start), DateTime.Parse(end)),
            DaysInterval = daysInterval,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        var result = _recurringExecutionService.CalculateNextExecutionTime(configuration);

        // Assert
        result.Should().HaveCount(expectedCount); 
    }

    [Theory]
    [InlineData("2024-01-01", "2024-01-31", new[] { DayOfWeek.Monday, DayOfWeek.Wednesday }, 1, 12)] 
    [InlineData("2024-01-01", "2024-01-31", new[] { DayOfWeek.Friday }, 1, 12)]
    public void CalculateNextExecutionTime_ShouldLimitExecutions_ForWeeklyFrequency(string start, string end, DayOfWeek[] daysOfWeek, int weekInterval, int expectedCount)
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = DateTime.Parse(start),
            TimeInterval = new LimitsTimeInterval(DateTime.Parse(start), DateTime.Parse(end)),
            DaysOfWeek = daysOfWeek.ToList(),
            WeekInterval = weekInterval,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        var result = _recurringExecutionService.CalculateNextExecutionTime(configuration);

        // Assert
        result.Should().HaveCount(expectedCount); 
    }
}
