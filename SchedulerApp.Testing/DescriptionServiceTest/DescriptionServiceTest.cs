using System.Globalization;
using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Models.ValueObjects;


namespace SchedulerApp.Testing.DescriptionServiceTest;

public class DescriptionServiceTests
{
    private readonly DescriptionService _descriptionService;
    CustomStringLocalizer localizer = new CustomStringLocalizer();

    public DescriptionServiceTests()
    {
        _descriptionService = new DescriptionService(localizer);
    }


    [Fact]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForOnceScheduler()
    {
        // Arrange
        var configuration = new OnceSchedulerConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 01, 01),
            ConfigurationDateTime = new DateTime(2024, 01, 01, 9, 0, 0),
        };
        var executionTime = new DateTime(2024, 01, 01, 9, 0, 0);

        // Set culture to en-GB
        CultureInfo.CurrentUICulture = new CultureInfo("en-GB");

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
            HourTimeRange = new HourTimeRange(onceAt),
            Culture = CultureOptions.en_GB
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
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Culture = CultureOptions.en_GB

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
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Culture = CultureOptions.en_GB
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
        act.Should().Throw<NullReferenceException>();
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
    public void GenerateDescription_ForFirstMondayMonthly()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 01, 01),
            DayOptions = DayOptions.First,
            MonthFrequency = 1,
            Culture = CultureOptions.en_GB,
            WeekOption = WeekOptions.Monday
        };
        CultureInfo.CurrentCulture = new CultureInfo("en-GB");

        var executionTime = new DateTime(2024, 01, 01, 13, 00, 00);
        var descriptionService = new DescriptionService(localizer);

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
            Culture = CultureOptions.en_GB,
            WeekOption = WeekOptions.Wednesday
        };
        var executionTime = new DateTime(2024, 01, 10, 04, 00, 00);
        var descriptionService = new DescriptionService(localizer);

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
            WeekOption = WeekOptions.Friday,
            Culture = CultureOptions.en_GB

        };
        var executionTime = new DateTime(2024, 01, 19, 15, 00, 00);
        var descriptionService = new DescriptionService(localizer);

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
        var descriptionService = new DescriptionService(localizer);

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
            WeekOption = WeekOptions.WeekendDay,
            Culture = CultureOptions.en_GB
        };
        var executionTime = new DateTime(2024, 01, 27, 21, 30, 00);
        var descriptionService = new DescriptionService(localizer);

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
            WeekOption = WeekOptions.Weekday,
            Culture = CultureOptions.en_GB,

        };
        var executionTime = new DateTime(2024, 01, 01, 09, 00, 00);
        var descriptionService = new DescriptionService(localizer);

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
        var descriptionService = new DescriptionService(localizer);

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
        var descriptionService = new DescriptionService(localizer);

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
            MonthFrequency = 6,
            Culture = CultureOptions.en_GB
        };
        var executionTime = new DateTime(2024, 01, 28, 10, 00, 00);
        var descriptionService = new DescriptionService(localizer);

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
            Culture = CultureOptions.en_GB,
            WeekOption = WeekOptions.WeekendDay
        };
        var executionTime = new DateTime(2024, 01, 06, 10, 00, 00);
        var descriptionService = new DescriptionService(localizer);

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
            WeekOption = WeekOptions.AnyDay,
            Culture = CultureOptions.en_GB
        };


        var executionTime = new DateTime(2024, 01, 01, 08, 00, 00);
        var descriptionService = new DescriptionService(localizer);

        // Act
        var description = descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs on day 1 of every 1 month(s). Schedule will be used on 01/01/2024 at 08:00 starting on 01/01/2024.");
    }

    [Fact]
    public void GenerateDescription_ForWeeklyFrequency_WithCultureEnUs()
    {

        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 07, 15),
            WeekInterval = 1,
            DaysOfWeek =  [DayOfWeek.Monday, DayOfWeek.Wednesday],
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Culture = CultureOptions.en_US
        };
        var executionTime = new DateTime(2024, 07, 15, 9, 0, 0);
        var descriptionService = new DescriptionService(localizer);

        // Act
        var description = descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs every 1 week(s) on Monday, Wednesday. Schedule will be used on 7/15/2024 at 9:00 AM starting on 7/15/2024.");
    }

    [Fact]
    public void GenerateDescription_ForWeeklyFrequency_WithCultureEnGb()
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 07, 15),
            WeekInterval = 1,
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Culture = CultureOptions.en_GB
        };
        var executionTime = new DateTime(2024, 07, 15, 9, 0, 0);
        var descriptionService = new DescriptionService(localizer);

        // Act
        var description = descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs every 1 week(s) on Monday, Wednesday. Schedule will be used on 15/07/2024 at 09:00 starting on 15/07/2024.");
    }

    [Fact]
    public void GenerateDescription_ForWeeklyFrequency_WithCultureEsEs()
    {
        // Arrange
        var config = new WeeklyFrequencyConfiguration
        {
            IsEnabled = true,
            CurrentDate = new DateTime(2024, 07, 15),
            WeekInterval = 1,
            DaysOfWeek = [DayOfWeek.Monday, DayOfWeek.Wednesday],
            HourTimeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0)),
            Culture = CultureOptions.es_ES
        };
        var executionTime = new DateTime(2024, 07, 15, 9, 0, 0);
        var descriptionService = new DescriptionService(localizer);

        // Act
        var description = descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Ocurre cada 1 semana(s) el lunes, miércoles. El horario se utilizará el 15/07/2024 a las 9:00 a partir del 15/07/2024.");
    }
   

    [Fact]
    public void GenerateDescription_ForMonthlyFrequency_WithCultureEnUs()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 05, 15),
            DayOptions = DayOptions.Third,
            MonthFrequency = 1,
            WeekOption = WeekOptions.Friday,
            Culture = CultureOptions.en_US
        };
        var executionTime = new DateTime(2024, 05, 17, 15, 00, 00);

        // Act
        var description = _descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs on the third Friday of every 1 month(s). Schedule will be used on 5/17/2024 at 3:00 PM starting on 5/15/2024.");
    }

    [Fact]
    public void GenerateDescription_ForMonthlyFrequency_WithCultureEnGb()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 05, 15),
            DayOptions = DayOptions.Third,
            MonthFrequency = 1,
            WeekOption = WeekOptions.Friday,
            Culture = CultureOptions.en_GB
        };
        var executionTime = new DateTime(2024, 05, 17, 15, 00, 00);

        // Act
        var description = _descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs on the third Friday of every 1 month(s). Schedule will be used on 17/05/2024 at 15:00 starting on 15/05/2024.");
    }

    [Fact]
    public void GenerateDescription_ForMonthlyFrequency_WithCultureEsEs()
    {
        // Arrange
        var config = new MonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 05, 15),
            DayOptions = DayOptions.Third,
            MonthFrequency = 1,
            WeekOption = WeekOptions.Friday,
            Culture = CultureOptions.es_ES
        };
        var executionTime = new DateTime(2024, 05, 17, 15, 00, 00);

        // Act
        var description = _descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Ocurre el tercer viernes de cada 1 mes(es). El horario se utilizará el 17/05/2024 a las 15:00 a partir del 15/05/2024.");
    }

    [Fact]
    public void GenerateDescription_ForSpecificDayMonthly_WithCultureEnUs()
    {
        // Arrange
        var config = new SpecificDayMonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 06, 20),
            SpecificDay = 30,
            MonthFrequency = 2,
            Culture = CultureOptions.en_US
        };
        var executionTime = new DateTime(2024, 06, 30, 10, 00, 00);

        // Act
        var description = _descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs on day 30 of every 2 month(s). Schedule will be used on 6/30/2024 at 10:00 AM starting on 6/20/2024.");
    }

    [Fact]
    public void GenerateDescription_ForSpecificDayMonthly_WithCultureEnGb()
    {
        // Arrange
        var config = new SpecificDayMonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 06, 20),
            SpecificDay = 30,
            MonthFrequency = 2,
            Culture = CultureOptions.en_GB
        };
        var executionTime = new DateTime(2024, 06, 30, 10, 00, 00);

        // Act
        var description = _descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Occurs on day 30 of every 2 month(s). Schedule will be used on 30/06/2024 at 10:00 starting on 20/06/2024.");
    }

    [Fact]
    public void GenerateDescription_ForSpecificDayMonthly_WithCultureEsEs()
    {
        // Arrange
        var config = new SpecificDayMonthlySchedulerConfiguration
        {
            CurrentDate = new DateTime(2024, 06, 20),
            SpecificDay = 30,
            MonthFrequency = 2,
            Culture = CultureOptions.es_ES
        };
        var executionTime = new DateTime(2024, 06, 30, 10, 00, 00);

        // Act
        var description = _descriptionService.GenerateDescription(config, executionTime);

        // Assert
        description.Should().Be("Ocurre el día 30 de cada 2 mes(es). El horario se utilizará el 30/06/2024 a las 10:00 a partir del 20/06/2024.");
    }


}



