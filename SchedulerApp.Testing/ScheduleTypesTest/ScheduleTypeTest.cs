using FluentAssertions;
using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Models.ValueObjects;
using SchedulerApplication.Services.DateCalculator;
using SchedulerApplication.Services.Description;
using SchedulerApplication.Services.ExecutionTime;
using SchedulerApplication.Services.HourCalculator;
using SchedulerApplication.Services.ScheduleTypes;

namespace SchedulerApp.Testing.ScheduleTypesTest;

    public class ScheduleTypeTest
    {
    private readonly ScheduleTypeFactory _factory;
    private readonly IDescriptionService _descriptionService;
    private readonly IExecutionTimeGenerator _timeGenerator;

    public ScheduleTypeTest()
    {
        _descriptionService = new DescriptionService();
        _timeGenerator = new ExecutionTimeGenerator(
            new OnceDateCalculator(),
            new DailyDateCalculator(),
            new WeeklyDateCalculator(),
            new HourCalculatorService());
        _factory = new ScheduleTypeFactory(_descriptionService, _timeGenerator);
    }

    [Fact]
    public void CreateSchedule_ShouldReturnCorrectExecution_WhenOnceSchedulerConfigurationIsCorrect()
    {

        //Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            ConfigurationDateTime = new DateTime(2024, 07, 15),
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 02, 09, 00, 00)
        };

        //Act
        var scheduleType = _factory.CreateScheduleType(configuration);
        var executionTimes = scheduleType.GetNextExecutionTimes(configuration);

        //Assert
        scheduleType.Should().BeOfType<ScheduleTypeOnce>();
        executionTimes.Should().HaveCount(1);
        executionTimes[0].ExecutionTime.Should().Be(new DateTime(2024, 07, 15));
        executionTimes[0].Description.Should().Be("Occurs once. Schedule will be used on 15/07/2024 at 00:00 starting on 02/01/2024.");
    }

    [Fact]
    public void CreateSchedule_ShouldReturnCorrectExecution_WhenDailyFrequencyConfigurationIsCorrect()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            CurrentDate = new DateTime(2024, 07, 15, 09, 02, 21),
            HourlyInterval = 2,
            HourTimeRange = new HourTimeRange(new TimeSpan(21, 00, 00), new TimeSpan(23, 00, 00)),
            IsEnabled = true,
            Limits = new LimitsTimeInterval(new DateTime(2024, 07, 15), new DateTime(2024, 07, 17))
        };

        // Act
        var scheduleType = _factory.CreateScheduleType(configuration);
        var executionTimes = scheduleType.GetNextExecutionTimes(configuration);

        // Assert
        scheduleType.Should().BeOfType<ScheduleTypeRecurring>();
        executionTimes.Should().HaveCount(12);

        var expectedTimes = new List<DateTime>
        {
            new DateTime(2024, 07, 15, 21, 00, 00),
            new DateTime(2024, 07, 15, 23, 00, 00),
            new DateTime(2024, 07, 16, 21, 00, 00),
            new DateTime(2024, 07, 16, 23, 00, 00),
            new DateTime(2024, 07, 17, 21, 00, 00),
            new DateTime(2024, 07, 17, 23, 00, 00)
        };

        for (int i = 0; i < expectedTimes.Count; i++)
        {
            executionTimes[i].ExecutionTime.Should().Be(expectedTimes[i]);
            executionTimes[i].Description.Should().Be($"Occurs every day at {expectedTimes[i]:HH:mm} from {configuration.HourTimeRange.StartHour.Hours} to {configuration.HourTimeRange.EndHour.Hours} . Schedule will be used on {expectedTimes[i]:dd/MM/yyyy} at {expectedTimes[i]:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
        }
    }
}

