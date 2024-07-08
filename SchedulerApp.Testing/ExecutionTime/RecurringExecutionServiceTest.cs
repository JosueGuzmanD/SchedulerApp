using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Common.Validator;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Services.ExecutionTime;
using SchedulerApplication.Services.HourCalculator;
using SchedulerApplication.Services.Interfaces;
using SchedulerApplication.Services.WeekCalculator;
using SchedulerApplication.ValueObjects;

public class RecurringExecutionServiceTests
{
    private readonly IRecurringExecutionService _recurringExecutionService;

    public RecurringExecutionServiceTests()
    {
        var validatorService = new ConfigurationValidator();
        var hourlyExecutionCalculatorService = new HourlyExecutionCalculatorService();
        var weeklyExecutionCalculatorService = new WeeklyExecutionCalculatorService(hourlyExecutionCalculatorService);

        _recurringExecutionService = new RecurringExecutionService(
            validatorService,
            hourlyExecutionCalculatorService,
            weeklyExecutionCalculatorService);
    }

    [Fact]
    public void CalculateNextExecutionTimes_Weekly_ShouldReturnCorrectTimes()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 01, 01), new DateTime(2024, 12, 31)),
            WeekInterval = 1,
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday },
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, DailyHourFrequency.Recurrent)
        };

        // Act
        var result = _recurringExecutionService.CalculateNextExecutionTimes(configuration);

        // Assert
        result.Should().HaveCount(12);
        result[0].Should().Be(new DateTime(2024, 01, 01, 09, 00, 00));
        result[1].Should().Be(new DateTime(2024, 01, 01, 10, 00, 00));
        result[2].Should().Be(new DateTime(2024, 01, 01, 11, 00, 00));
        result[3].Should().Be(new DateTime(2024, 01, 01, 12, 00, 00));
        result[4].Should().Be(new DateTime(2024, 01, 01, 13, 00, 00));
        result[5].Should().Be(new DateTime(2024, 01, 01, 14, 00, 00));
        result[6].Should().Be(new DateTime(2024, 01, 01, 15, 00, 00));
        result[7].Should().Be(new DateTime(2024, 01, 01, 16, 00, 00));
        result[8].Should().Be(new DateTime(2024, 01, 01, 17, 00, 00));
        result[9].Should().Be(new DateTime(2024, 01, 08, 09, 00, 00));
        result[10].Should().Be(new DateTime(2024, 01, 08, 10, 00, 00));
        result[11].Should().Be(new DateTime(2024, 01, 08, 11, 00, 00));
    }
}
