using FluentAssertions;
using SchedulerApplication.Models.SchedulerConfigurations;
using SchedulerApplication.Services.Description;

namespace SchedulerApp.Testing.DescriptionServiceTest;

public class DescriptionServiceTest
{
    [Theory]
    [InlineData("2024-06-20T20:12:32")]
    [InlineData("2004-05-21T01:13:32")]
    [InlineData("2014-03-22T23:12:32")]
    public void GenerateDescription_ShouldReturnCorrectDescription_ForDifferentDates(string date)
    {
        //Arrange
        var dateTime = DateTime.Parse(date);
        var configuration = new OnceSchedulerConfiguration();
        var service = new DescriptionService();

        //Act
        var result = service.GenerateDescription(configuration, dateTime);

        //Assert
        result.Should().BeEquivalentTo($"Occurs Once. Schedule will be used on {dateTime:dd/MM/yyyy} at {dateTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
    }

    [Fact]
    public void GenerateDescription_ShouldReturnDescription_WhenDaysIntervalIs1()
    {
        //Arrange
        var configuration = new RecurringSchedulerConfiguration()
        { DaysInterval = 1 };
        var service = new DescriptionService();
        var dateTime = new DateTime(2026, 01, 29, 03, 10, 39);

        //Act
        var result = service.GenerateDescription(configuration, dateTime);

        //Assert
        result.Should().BeEquivalentTo($"Occurs every day. Schedule will be used on {dateTime:dd/MM/yyyy} at {dateTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");

    }

    [Fact]
    public void GenerateDescription_ShouldReturnException_WhenDaysIntervalIs0()
    {
        //Arrange
        var configuration = new RecurringSchedulerConfiguration()
        { DaysInterval = 0 };
        var service = new DescriptionService();
        var dateTime = new DateTime(2028, 06, 09, 23, 21, 21);

        //Act
        Action act = () => service.GenerateDescription(configuration, dateTime);

        //Assert
        act.Should().Throw<IndexOutOfRangeException>();
    }

    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(65)]
    public void GenerateDescription_ShouldReturnDescription_WhenDaysIntervalIsMajor1(int DaysInterval)
    {
        //Arrange
        var configuration = new RecurringSchedulerConfiguration()
        { DaysInterval = DaysInterval };
        var service = new DescriptionService();
        var dateTime = new DateTime(2028, 04, 5, 23, 23, 21, 21);

        //Act
        var result = service.GenerateDescription(configuration, dateTime);

        //Assert
        result.Should().BeEquivalentTo($"Occurs every {configuration.DaysInterval} days. Schedule will be used on {dateTime:dd/MM/yyyy} at {dateTime:HH:mm} starting on {configuration.CurrentDate:dd/MM/yyyy}.");
    }
}

