using FluentAssertions;
using SchedulerApplication.Services.WeekCalculator;

namespace SchedulerApp.Testing;

public class WeeklyExecutionServiceTests
{
    [Theory]
    [InlineData(2024, 6, 1, DayOfWeek.Monday, "2024-06-03")]
    [InlineData(2024, 6, 1, DayOfWeek.Friday, "2024-06-07")]
    public void CalculateWeeklyDates_ShouldReturnCorrectNextDay(int year, int month, int day, DayOfWeek dayOfWeek,
        string expected)
    {
        // Arrange
        DateTime initialDate = new DateTime(year, month, day);
        var service = new WeekCalculatorService();

        // Act
        var result = service.CalculateWeeklyDates(initialDate, new List<DayOfWeek> { dayOfWeek }, 1);

        // Assert
        result.First().Should().Be(DateTime.Parse(expected));
    }

    [Fact]
    public void CalculateWeeklyDates_ShouldReturnCorrectDates()
    {
        // Arrange
        DateTime initialDate = new DateTime(2024, 6, 1);
        var service = new WeekCalculatorService();

        // Act
        var result = service.CalculateWeeklyDates(initialDate,
            new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday }, 1);

        // Assert
        result.Should().HaveCount(6);
        result[0].Should().Be(new DateTime(2024, 6, 3));
        result[1].Should().Be(new DateTime(2024, 6, 5));
        result[2].Should().Be(new DateTime(2024, 6, 10));
        result[3].Should().Be(new DateTime(2024, 6, 12));
        result[4].Should().Be(new DateTime(2024, 6, 17));
        result[5].Should().Be(new DateTime(2024, 6, 19));
    }
}

