using FluentAssertions;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Description;
using SchedulerApplication.ValueObjects;

namespace SchedulerApp.Testing.DescriptionServiceTest;

public class DescriptionServiceTests
{
    private readonly DescriptionService _descriptionService;

    public DescriptionServiceTests()
    {
        _descriptionService = new DescriptionService();
    }

    [Theory]
    [InlineData(0)]
    public void GenerateDescription_ShouldThrowException_WhenDaysIntervalIsZero_DailyFrequency(int daysInterval)
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            DaysInterval = daysInterval,
            TimeInterval = new LimitsTimeInterval(DateTime.Now)
        };

        // Act
        Action act = () => _descriptionService.GenerateDescription(configuration, DateTime.Now);

        // Assert
        act.Should().Throw<IndexOutOfRangeException>().WithMessage("Days interval cannot be 0");
    }

    [Theory]
    [InlineData(0)]
    public void GenerateDescription_ShouldThrowException_WhenDaysIntervalIsZero_WeeklyFrequency(int daysInterval)
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            DaysInterval = daysInterval,
            TimeInterval = new LimitsTimeInterval(DateTime.Now)
        };

        // Act
        Action act = () => _descriptionService.GenerateDescription(configuration, DateTime.Now);

        // Assert
        act.Should().Throw<IndexOutOfRangeException>().WithMessage("Days interval cannot be 0");
    }

    [Fact]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForOnceScheduler()
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            TimeInterval = new LimitsTimeInterval(DateTime.Now)
        };
        var executionTime = DateTime.Now;

        // Act
        var result = _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        result.Should().Be($"Occurs Once. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.TimeInterval.LimitStartDateTime:dd/MM/yyyy}.");
    }

    [Theory]
    [InlineData(1, "Occurs every day")]
    [InlineData(2, "Occurs every 2 days")]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForDailyFrequency(int daysInterval, string expectedDescription)
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            DaysInterval = daysInterval,
            TimeInterval = new LimitsTimeInterval(DateTime.Now)
        };
        var executionTime = DateTime.Now;

        // Act
        var result = _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        result.Should().Be($"{expectedDescription}. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.TimeInterval.LimitStartDateTime:dd/MM/yyyy}.");
    }

    [Theory]
    [InlineData(1, "Occurs every week")]
    [InlineData(2, "Occurs every 2 weeks")]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForWeeklyFrequency(int daysInterval, string expectedDescription)
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            DaysInterval = daysInterval,
            TimeInterval = new LimitsTimeInterval(DateTime.Now)
        };
        var executionTime = DateTime.Now;

        // Act
        var result = _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        result.Should().Be($"{expectedDescription}. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.TimeInterval.LimitStartDateTime:dd/MM/yyyy}.");
    }

    [Fact]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForWeeklyFrequency_WithMultipleDays()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            DaysInterval = 2,
            TimeInterval = new LimitsTimeInterval(DateTime.Now),
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday }
        };
        var executionTime = DateTime.Now;

        // Act
        var result = _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        result.Should().Be($"Occurs every 2 weeks. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.TimeInterval.LimitStartDateTime:dd/MM/yyyy}.");
    }
}