using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.Services.HourCalculator;
using SchedulerApplication.ValueObjects;

namespace SchedulerApp.Testing.HourCalculatorServiceTest;

public class HourCalculatorServiceTests
{
    [Fact]
    public void CalculateHour_ShouldThrowArgumentOutOfRangeException_ForUnsupportedFrequency()
    {
        // Arrange
        var baseDate = new DateTime(2021,05,06,02,32,21);
        var timeRange = new HourTimeRange(new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), 1, (DailyHourFrequency)999);

        var service = new HourCalculatorService();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => service.CalculateHour(baseDate, timeRange));
    }

    [Theory]
    [InlineData("2020-01-01","2:00:00")]
    [InlineData("2040-12-21", "12:00:20")]
    [InlineData("2012-12-21", "12:00:20")]
    [InlineData("2022-12-21", "14:00:20")]
    [InlineData("2020-12-21", "21:00:20")]
    public void CalculateHour_ShouldThrowOneDate_IfDailyFrequencyIsOnce(string basedate,string hourtimeRange)
    {

        //Arrange
        var service= new HourCalculatorService();
        var date= DateTime.Parse(basedate);
        var hourTimeRange = new HourTimeRange(TimeSpan.Parse(hourtimeRange));

        var expectedlist = new List<DateTime> { date.Date.Add(hourTimeRange.StartHour)};
        //Act
        var result= service.CalculateHour(date, hourTimeRange);

        //Assert
        result.Should().BeEquivalentTo(expectedlist);
    }
    [Fact]
    public void CalculateHour_ShouldHandleEndOfDay_ForOnceFrequency()
    {
        // Arrange
        var service = new HourCalculatorService();
        var baseDate = new DateTime(2024, 12, 21);
        var hourTimeRange = new HourTimeRange(new TimeSpan(23, 59, 59));

        // Act
        var result = service.CalculateHour(baseDate, hourTimeRange);

        // Assert
        result.Should().ContainSingle().Which.Should().Be(baseDate.Date.Add(new TimeSpan(23, 59, 59)));
    }
    [Fact]
    public void CalculateHour_ShouldHandleStartOfDay_ForOnceFrequency()
    {
        // Arrange
        var service = new HourCalculatorService();
        var baseDate = new DateTime(2024, 12, 21);
        var hourTimeRange = new HourTimeRange(new TimeSpan(0, 0, 0));

        // Act
        var result = service.CalculateHour(baseDate, hourTimeRange);

        // Assert
        result.Should().ContainSingle().Which.Should().Be(baseDate.Date.Add(new TimeSpan(0, 0, 0)));
    }

    [Theory]
    [InlineData("2024-12-12T00:00:00", "08:00:00", "10:00:00", 1, new[] { "2024-12-12T08:00:00", "2024-12-12T09:00:00", "2024-12-12T10:00:00" })]
    [InlineData("2024-12-12T00:00:00", "09:00:00", "17:00:00", 3, new[] { "2024-12-12T09:00:00", "2024-12-12T12:00:00", "2024-12-12T15:00:00" })]
    [InlineData("2024-12-12T00:00:00", "10:00:00", "18:00:00", 2, new[] { "2024-12-12T10:00:00", "2024-12-12T12:00:00", "2024-12-12T14:00:00", "2024-12-12T16:00:00", "2024-12-12T18:00:00" })]
    [InlineData("2024-12-12T00:00:00", "11:00:00", "14:00:00", 1, new[] { "2024-12-12T11:00:00", "2024-12-12T12:00:00", "2024-12-12T13:00:00", "2024-12-12T14:00:00" })]
    public void CalculateHour_ShouldReturnCorrectHourlyList_ForRecurrentFrequency(string baseDateStr, string startHourStr, string endHourStr, int interval, string[] expectedDatesStr)
    {
        // Arrange
        var baseDate = DateTime.Parse(baseDateStr);
        var startHour = TimeSpan.Parse(startHourStr);
        var endHour = TimeSpan.Parse(endHourStr);
        var expectedList = expectedDatesStr.Select(DateTime.Parse).ToList();

        var timeRange = new HourTimeRange(startHour, endHour, interval, DailyHourFrequency.Recurrent);
        var service = new HourCalculatorService();

        // Act
        var result = service.CalculateHour(baseDate, timeRange);

        // Assert
        result.Should().BeEquivalentTo(expectedList);
    }

    [Fact]
    public void CalculateHour_ShouldHandleMidnightTransition_ForRecurrentFrequency()
    {
        // Arrange
        var startHour = new TimeSpan(23, 0, 0);
        var baseDate = new DateTime(2024, 12, 12);
        var endHour = new TimeSpan(2, 0, 0);
        var timeRange = new HourTimeRange(startHour, endHour, 1, DailyHourFrequency.Recurrent);
        var service = new HourCalculatorService();

        var expectedList = new List<DateTime>
        {
            baseDate.Date.Add(startHour),
            baseDate.Date.Add(startHour).AddHours(1),
            baseDate.Date.Add(startHour).AddHours(2),
        };

        // Act
        var result = service.CalculateHour(baseDate, timeRange);

        // Assert
        result.Should().BeEquivalentTo(expectedList);
    }

    [Fact]
    public void CalculateHour_ShouldHandleSingleExecution_WhenStartHourEqualsEndHour_ForRecurrentFrequency()
    {
        // Arrange
        var startHour = new TimeSpan(14, 0, 0);
        var baseDate = new DateTime(2024, 12, 12);
        var endHour = new TimeSpan(14, 0, 0);
        var timeRange = new HourTimeRange(startHour, endHour, 1, DailyHourFrequency.Recurrent);
        var service = new HourCalculatorService();

        var expectedList = new List<DateTime>
        {
            baseDate.Date.Add(startHour)
        };

        // Act
        var result = service.CalculateHour(baseDate, timeRange);

        // Assert
        result.Should().BeEquivalentTo(expectedList);
    }


}

