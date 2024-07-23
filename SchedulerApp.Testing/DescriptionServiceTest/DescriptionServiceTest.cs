using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Models.ValueObjects;
using SchedulerApplication.Services.Description;

namespace SchedulerApp.Testing.DescriptionServiceTest;

public class DescriptionServiceTests
{
    private readonly DescriptionService _descriptionService;

    public DescriptionServiceTests()
    {
        _descriptionService = new DescriptionService();
    }

    [Fact]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForOnceScheduler()
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            ConfigurationDateTime = new DateTime(2024, 01, 01, 9, 0, 0)
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        var result = _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        result.Should().Be($"Occurs once. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
    }

    [Fact]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForDailyFrequency_Once_09_00()
    {
        // Arrange
        var onceAt = new TimeSpan(9, 0, 0);
        var configuration = new DailyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(onceAt)
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        var result = _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        result.Should().Be($"Occurs once at 09:00. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
    }

    [Fact]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForDailyFrequency_Once_10_00()
    {
        // Arrange
        var onceAt = new TimeSpan(10, 0, 0);
        var configuration = new DailyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(onceAt)
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        var result = _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        result.Should().Be($"Occurs once at 10:00. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
    }

    [Fact]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForDailyFrequency_Recurrent()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0))
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        var result = _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        result.Should().Be($"Occurs every day from 09:00 to 17:00. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
    }

    [Theory]
    [InlineData(1, "Occurs every 1 week(s) on Monday, Wednesday")]
    [InlineData(2, "Occurs every 2 week(s) on Monday, Wednesday")]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForWeeklyFrequency(int weekInterval, string expectedDescription)
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            WeekInterval = weekInterval,
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0))
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        var result = _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        result.Should().Be($"{expectedDescription}. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
    }

    [Fact]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForWeeklyFrequency_WithMultipleDays()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            WeekInterval = 2,
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0))
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        var result = _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        result.Should().Be($"Occurs every 2 week(s) on Monday, Wednesday. Schedule will be used on {executionTime:dd/MM/yyyy} at {executionTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
    }

    [Fact]
    public void GenerateDescription_DailyConfig_NullHourTimeRange_ShouldThrowArgumentNullException()
    {
        // Arrange
        var configuration = new DailyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            HourTimeRange = null
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        Action act = () => _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GenerateDescription_WeeklyConfig_NullHourTimeRange_ShouldThrowArgumentNullException()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            WeekInterval = 1,
            DaysOfWeek = [DayOfWeek.Monday],
            HourTimeRange = null
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        Action act = () => _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GenerateDescription_WeeklyConfig_NullDaysOfWeek_ShouldThrowArgumentNullException()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            WeekInterval = 1,
            DaysOfWeek = null,
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0))
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        Action act = () => _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GenerateDescription_WeeklyConfig_EmptyDaysOfWeek_ShouldThrowArgumentNullException()
    {
        // Arrange
        var configuration = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            WeekInterval = 1,
            DaysOfWeek = [],
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0))
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Act
        Action act = () => _descriptionService.GenerateDescription(configuration, executionTime);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GenerateDescription_ForFirstMondayMonthly()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.First,
            MonthFrequency = 1,
            WeekOption = WeekOptions.Monday
        };
        var executionTime = new DateTime(2024, 01, 01, 13, 00, 00);
        var descriptionService = new DescriptionService();

        // Act
        var description = descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs on the first Monday of every 1 month(s). Schedule will be used on 01/01/2024 at 13:00 starting on 01/01/2024.");
    }
    [Fact]
    public void GenerateDescription_ForSecondWednesdayMonthly()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.Second,
            MonthFrequency = 1,
            WeekOption = WeekOptions.Wednesday
        };
        var executionTime = new DateTime(2024, 01, 10, 04, 00, 00);
        var descriptionService = new DescriptionService();

        // Act
        var description = descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs on the second Wednesday of every 1 month(s). Schedule will be used on 10/01/2024 at 04:00 starting on 01/01/2024.");
    }
    [Fact]
    public void GenerateDescription_ForThirdFridayMonthly()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.Third,
            MonthFrequency = 1,
            WeekOption = WeekOptions.Friday
        };
        var executionTime = new DateTime(2024, 01, 19, 15, 00, 00);
        var descriptionService = new DescriptionService();

        // Act
        var description = descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs on the third Friday of every 1 month(s). Schedule will be used on 19/01/2024 at 15:00 starting on 01/01/2024.");
    }
    [Fact]
    public void GenerateDescription_ForFourthSaturdayMonthly()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.Fourth,
            MonthFrequency = 1,
            WeekOption = WeekOptions.Saturday
        };
        var executionTime = new DateTime(2024, 01, 27, 21, 30, 00);
        var descriptionService = new DescriptionService();

        // Act
        var description = descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs on the fourth Saturday of every 1 month(s). Schedule will be used on 27/01/2024 at 21:30 starting on 01/01/2024.");
    }
    [Fact]
    public void GenerateDescription_ForLastWeekendDayMonthly()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.Last,
            MonthFrequency = 1,
            WeekOption = WeekOptions.WeekendDay
        };
        var executionTime = new DateTime(2024, 01, 27, 21, 30, 00);
        var descriptionService = new DescriptionService();

        // Act
        var description = descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs on the last weekend day of every 1 month(s). Schedule will be used on 27/01/2024 at 21:30 starting on 01/01/2024.");
    }
    [Fact]
    public void GenerateDescription_ForFirstWeekdayMonthly()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.First,
            MonthFrequency = 1,
            WeekOption = WeekOptions.Weekday
        };
        var executionTime = new DateTime(2024, 01, 01, 09, 00, 00);
        var descriptionService = new DescriptionService();

        // Act
        var description = descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs on the first weekday of every 1 month(s). Schedule will be used on 01/01/2024 at 09:00 starting on 01/01/2024.");
    }
    [Fact]
    public void GenerateDescription_ForSpecificDay15EveryThreeMonths()
    {
        // Arrange
        var config = new SpecificDayMonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            SpecificDay = 15,
            MonthFrequency = 3
        };
        var executionTime = new DateTime(2024, 01, 15, 12, 00, 00);
        var descriptionService = new DescriptionService();

        // Act
        var description = descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs on day 15 of every 3 month(s). Schedule will be used on 15/01/2024 at 12:00 starting on 01/01/2024.");
    }
    [Fact]
    public void GenerateDescription_ForSpecificDay5EveryTwoMonths()
    {
        // Arrange
        var config = new SpecificDayMonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            SpecificDay = 5,
            MonthFrequency = 2
        };
        var executionTime = new DateTime(2024, 01, 05, 09, 00, 00);
        var descriptionService = new DescriptionService();

        // Act
        var description = descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs on day 5 of every 2 month(s). Schedule will be used on 05/01/2024 at 09:00 starting on 01/01/2024.");
    }
    [Fact]
    public void GenerateDescription_ForSpecificDay28EverySixMonths()
    {
        // Arrange
        var config = new SpecificDayMonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            SpecificDay = 28,
            MonthFrequency = 6
        };
        var executionTime = new DateTime(2024, 01, 28, 10, 00, 00);
        var descriptionService = new DescriptionService();

        // Act
        var description = descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs on day 28 of every 6 month(s). Schedule will be used on 28/01/2024 at 10:00 starting on 01/01/2024.");
    }
    [Fact]
    public void GenerateDescription_ForFirstWeekendDayMonthly()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.First,
            MonthFrequency = 1,
            WeekOption = WeekOptions.WeekendDay
        };
        var executionTime = new DateTime(2024, 01, 06, 10, 00, 00);
        var descriptionService = new DescriptionService();

        // Act
        var description = descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs on the first weekend day of every 1 month(s). Schedule will be used on 06/01/2024 at 10:00 starting on 01/01/2024.");
    }
    [Fact]
    public void GenerateDescription_ForFirstAnyDayMonthly()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.First,
            MonthFrequency = 1,
            WeekOption = WeekOptions.AnyDay
        };
        var executionTime = new DateTime(2024, 01, 01, 08, 00, 00);
        var descriptionService = new DescriptionService();

        // Act
        var description = descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs on day 1 of every 1 month(s). Schedule will be used on 01/01/2024 at 08:00 starting on 01/01/2024.");
    }


}



