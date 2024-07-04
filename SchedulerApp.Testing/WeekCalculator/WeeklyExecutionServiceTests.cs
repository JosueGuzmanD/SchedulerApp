using FluentAssertions;
using SchedulerApplication.Services.WeekCalculator;

namespace SchedulerApp.Testing.WeekCalculator;

public class WeeklyExecutionServiceTests
{
    private readonly WeekCalculatorService _service = new WeekCalculatorService();
    [Theory]
    [InlineData("2024-06-01", new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Wednesday }, 1,
        new string[] { "2024-06-03", "2024-06-05", "2024-06-10", "2024-06-12", "2024-06-17", "2024-06-19" })]
    [InlineData("2024-06-01", new DayOfWeek[] { DayOfWeek.Friday }, 2,
        new string[] { "2024-06-07", "2024-06-21", "2024-07-05" })]
    [InlineData("2024-06-01", new DayOfWeek[] { DayOfWeek.Sunday, DayOfWeek.Saturday }, 1,
        new string[] { "2024-06-02", "2024-06-08", "2024-06-09", "2024-06-15", "2024-06-16", "2024-06-22" })]
    public void CalculateWeeklyDates_ShouldReturnCorrectDates(string initialDateString, DayOfWeek[] daysOfWeek, int weekInterval, string[] expectedDatesStrings)
    {
        // Arrange
        var initialDate = DateTime.Parse(initialDateString);
        var expectedDates = expectedDatesStrings.Select(DateTime.Parse).ToList();

        // Act
        var result = _service.CalculateWeeklyDates(initialDate, daysOfWeek.ToList(), weekInterval);

        // Assert
        result.Should().BeEquivalentTo(expectedDates);
    }
    [Fact]
    public void CalculateWeeklyDates_ShouldReturnEmptyList_WhenNoDaysOfWeekProvided()
    {
        // Arrange
        var initialDate = new DateTime(2024, 06, 01);
        var daysOfWeek = new List<DayOfWeek>();

        // Act
        var result = _service.CalculateWeeklyDates(initialDate, daysOfWeek, 1);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void CalculateWeeklyDates_ShouldReturnCorrectDates_ForSingleDayOfWeek()
    {
        // Arrange
        var initialDate = new DateTime(2024, 06, 01);
        var daysOfWeek = new List<DayOfWeek> { DayOfWeek.Wednesday };
        var expectedDates = new List<DateTime>
        {
            new DateTime(2024, 06, 05),
            new DateTime(2024, 06, 12),
            new DateTime(2024, 06, 19)
        };

        // Act
        var result = _service.CalculateWeeklyDates(initialDate, daysOfWeek, 1);

        // Assert
        result.Should().BeEquivalentTo(expectedDates);
    }

    [Fact]
    public void CalculateWeeklyDates_ShouldHandleWeekIntervalOfTwo()
    {
        // Arrange
        var initialDate = new DateTime(2024, 06, 01);
        var daysOfWeek = new List<DayOfWeek> { DayOfWeek.Friday };
        var expectedDates = new List<DateTime>
        {
            new DateTime(2024, 06, 07),
            new DateTime(2024, 06, 21),
            new DateTime(2024, 07, 05)
        };

        // Act
        var result = _service.CalculateWeeklyDates(initialDate, daysOfWeek, 2);

        // Assert
        result.Should().BeEquivalentTo(expectedDates);
    }

    [Fact]
    public void CalculateWeeklyDates_ShouldReturnCorrectDates_ForMultipleDaysOfWeek()
    {
        // Arrange
        var initialDate = new DateTime(2024, 06, 01);
        var daysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday };
        var expectedDates = new List<DateTime>
        {
            new DateTime(2024, 06, 03),
            new DateTime(2024, 06, 05),
            new DateTime(2024, 06, 10),
            new DateTime(2024, 06, 12),
            new DateTime(2024, 06, 17),
            new DateTime(2024, 06, 19)
        };

        // Act
        var result = _service.CalculateWeeklyDates(initialDate, daysOfWeek, 1);

        // Assert
        result.Should().BeEquivalentTo(expectedDates);
    }
    [Fact]
    public void CalculateWeeklyDates_ShouldHandleInitialDateNotAtStartOfWeek()
    {
        // Arrange
        var initialDate = new DateTime(2024, 06, 04); 
        var daysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday };
        var expectedDates = new List<DateTime>
        {
            new DateTime(2024, 06, 05),
            new DateTime(2024, 06, 10),
            new DateTime(2024, 06, 12),
            new DateTime(2024, 06, 17),
            new DateTime(2024, 06, 19),
            new DateTime(2024, 06, 24)
        };

        // Act
        var result = _service.CalculateWeeklyDates(initialDate, daysOfWeek, 1);

        // Assert
        result.Should().BeEquivalentTo(expectedDates);
    }

    [Fact]
    public void CalculateWeeklyDates_ShouldHandleLeapYear()
    {
        // Arrange
        var initialDate = new DateTime(2024, 02, 28);
        var daysOfWeek = new List<DayOfWeek> { DayOfWeek.Friday, DayOfWeek.Saturday };
        var expectedDates = new List<DateTime>
        {
            new DateTime(2024, 03, 01),
            new DateTime(2024, 03, 02),
            new DateTime(2024, 03, 08),
            new DateTime(2024, 03, 09),
            new DateTime(2024, 03, 15),
            new DateTime(2024, 03, 16)
        };

        // Act
        var result = _service.CalculateWeeklyDates(initialDate, daysOfWeek, 1);

        // Assert
        result.Should().BeEquivalentTo(expectedDates);
    }

}

