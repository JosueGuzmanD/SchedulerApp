using FluentAssertions;
using SchedulerApplication.Services.WeekCalculator;

namespace SchedulerApp.Testing.WeekCalculator;

public class WeekCalculatorServiceTests
{
    private readonly WeekCalculatorService _service;

    public WeekCalculatorServiceTests()
    {
        _service = new WeekCalculatorService();
    }

    [Fact]
    public void CalculateWeeklyDates_ShouldReturnCorrectDates_ForSingleDay()
    {
        // Arrange
        var initialDate = new DateTime(2024, 01, 01); 
        var daysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday };
        var expectedDates = new List<DateTime>
        {
            new DateTime(2024, 01, 01),
            new DateTime(2024, 01, 08),
            new DateTime(2024, 01, 15)
        };

        // Act
        var result = _service.CalculateWeeklyDates(initialDate, daysOfWeek, 1);

        // Assert
        result.Should().BeEquivalentTo(expectedDates);
    }

    [Fact]
    public void CalculateWeeklyDates_ShouldHandleMultipleDaysOfWeek()
    {
        // Arrange
        var initialDate = new DateTime(2024, 01, 01); 
        var daysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday };
        var expectedDates = new List<DateTime>
        {
            new DateTime(2024, 01, 01),
            new DateTime(2024, 01, 03),
            new DateTime(2024, 01, 08),
            new DateTime(2024, 01, 10),
            new DateTime(2024, 01, 15),
            new DateTime(2024, 01, 17)
        };

        // Act
        var result = _service.CalculateWeeklyDates(initialDate, daysOfWeek, 1);

        // Assert
        result.Should().BeEquivalentTo(expectedDates);
    }

    [Theory]
    [InlineData("2024-01-02", DayOfWeek.Monday, "2024-01-08")]
    [InlineData("2024-01-03", DayOfWeek.Monday, "2024-01-08")]
    [InlineData("2024-01-04", DayOfWeek.Monday, "2024-01-08")]
    [InlineData("2024-01-05", DayOfWeek.Monday, "2024-01-08")]
    [InlineData("2024-01-06", DayOfWeek.Monday, "2024-01-08")]
    [InlineData("2024-01-07", DayOfWeek.Monday, "2024-01-08")]
    public void GetNextSpecificDay_ShouldReturnCorrectNextDate(string currentDateString, DayOfWeek targetDay, string expectedDateString)
    {
        // Arrange
        var currentDate = DateTime.Parse(currentDateString);
        var expectedDate = DateTime.Parse(expectedDateString);

        // Act
        var result = _service.GetType().GetMethod("GetNextSpecificDay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                               .Invoke(_service, new object[] { currentDate, targetDay });

        // Assert
        result.Should().Be(expectedDate);
    }

    [Fact]
    public void CalculateWeeklyDates_ShouldReturnEmptyList_WhenNoDaysOfWeekProvided()
    {
        // Arrange
        var initialDate = new DateTime(2024, 01, 01); 
        var daysOfWeek = new List<DayOfWeek>();

        // Act
        var result = _service.CalculateWeeklyDates(initialDate, daysOfWeek, 1);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void CalculateWeeklyDates_ShouldHandleLeapYear()
    {
        // Arrange
        var initialDate = new DateTime(2024, 02, 28);
        var daysOfWeek = new List<DayOfWeek> { DayOfWeek.Thursday, DayOfWeek.Friday };
        var expectedDates = new List<DateTime>
        {
            new DateTime(2024, 02, 29), 
            new DateTime(2024, 03, 01), 
            new DateTime(2024, 03, 07), 
            new DateTime(2024, 03, 08), 
            new DateTime(2024, 03, 14), 
            new DateTime(2024, 03, 15)  
        };

        // Act
        var result = _service.CalculateWeeklyDates(initialDate, daysOfWeek, 1);

        // Assert
        result.Should().BeEquivalentTo(expectedDates);
    }

    [Theory]
    [InlineData("2024-06-01", new[] { DayOfWeek.Monday, DayOfWeek.Wednesday }, 1,
        new string[] { "2024-06-03", "2024-06-05", "2024-06-10", "2024-06-12", "2024-06-17", "2024-06-19" })]
    [InlineData("2024-06-01", new[] { DayOfWeek.Friday }, 2,
        new string[] { "2024-06-07", "2024-06-21", "2024-07-05" })]
    [InlineData("2024-06-01", new[] { DayOfWeek.Sunday, DayOfWeek.Saturday }, 1,
        new string[] { "2024-06-02", "2024-06-08", "2024-06-09", "2024-06-15", "2024-06-16" })]
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
}
