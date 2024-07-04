using FluentAssertions;
using SchedulerApplication.Common.Enums;
using SchedulerApplication.ValueObjects;

namespace SchedulerApp.Testing.ValueObjects;

public class HourTimeRangeTests
{

    [Fact]
    public void Constructor_ShouldThrowException_WhenHourlyIntervalIsLessThanOrEqualToZero()
    {
        // Arrange
        var startHour = new TimeSpan(10, 0, 0);
        var endHour = new TimeSpan(12, 0, 0);

        // Act
        Action act = () => new HourTimeRange(startHour, endHour, 0, DailyHourFrequency.Recurrent);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("HourlyInterval must be greater than 0.");
    }

    [Fact]
    public void Constructor_ShouldInitializeProperties_WhenParametersAreValid()
    {
        // Arrange
        var startHour = new TimeSpan(10, 0, 0);
        var endHour = new TimeSpan(12, 0, 0);
        var hourlyInterval = 1;
        var frequency = DailyHourFrequency.Recurrent;

        // Act
        var range = new HourTimeRange(startHour, endHour, hourlyInterval, frequency);

        // Assert
        range.StartHour.Should().Be(startHour);
        range.EndHour.Should().Be(endHour);
        range.HourlyInterval.Should().Be(hourlyInterval);
        range.HourlyFrequency.Should().Be(frequency);
    }

    [Fact]
    public void ConstructorForOnce_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var startHour = new TimeSpan(10, 0, 0);

        // Act
        var range = new HourTimeRange(startHour);

        // Assert
        range.StartHour.Should().Be(startHour);
        range.HourlyFrequency.Should().Be(DailyHourFrequency.Once);
        range.EndHour.Should().Be(TimeSpan.Zero);
        range.HourlyInterval.Should().Be(0);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForEquivalentHourTimeRanges()
    {
        // Arrange
        var startHour = new TimeSpan(10, 0, 0);
        var endHour = new TimeSpan(12, 0, 0);
        var hourlyInterval = 1;
        var frequency = DailyHourFrequency.Recurrent;

        var range1 = new HourTimeRange(startHour, endHour, hourlyInterval, frequency);
        var range2 = new HourTimeRange(startHour, endHour, hourlyInterval, frequency);

        // Act & Assert
        range1.Equals(range2).Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_ForDifferentHourTimeRanges()
    {
        // Arrange
        var range1 = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent);
        var range2 = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0), 2, DailyHourFrequency.Recurrent);

        // Act & Assert
        range1.Equals(range2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameHashCode_ForEquivalentHourTimeRanges()
    {
        // Arrange
        var startHour = new TimeSpan(10, 0, 0);
        var endHour = new TimeSpan(12, 0, 0);
        var hourlyInterval = 1;
        var frequency = DailyHourFrequency.Recurrent;

        var range1 = new HourTimeRange(startHour, endHour, hourlyInterval, frequency);
        var range2 = new HourTimeRange(startHour, endHour, hourlyInterval, frequency);

        // Act & Assert
        range1.GetHashCode().Should().Be(range2.GetHashCode());
    }

    [Fact]
    public void OperatorEquals_ShouldReturnTrue_ForEquivalentHourTimeRanges()
    {
        // Arrange
        var startHour = new TimeSpan(10, 0, 0);
        var endHour = new TimeSpan(12, 0, 0);
        var hourlyInterval = 1;
        var frequency = DailyHourFrequency.Recurrent;

        var range1 = new HourTimeRange(startHour, endHour, hourlyInterval, frequency);
        var range2 = new HourTimeRange(startHour, endHour, hourlyInterval, frequency);

        // Act & Assert
        (range1 == range2).Should().BeTrue();
    }

    [Fact]
    public void OperatorNotEquals_ShouldReturnTrue_ForDifferentHourTimeRanges()
    {
        // Arrange
        var range1 = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent);
        var range2 = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0), 2, DailyHourFrequency.Recurrent);

        // Act & Assert
        (range1 != range2).Should().BeTrue();
    }

    [Fact]
    public void EqualsObject_ShouldReturnTrue_ForEquivalentHourTimeRanges()
    {
        // Arrange
        var range1 = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent);
        object range2 = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent);

        // Act & Assert
        range1.Equals(range2).Should().BeTrue();
    }

    [Fact]
    public void EqualsObject_ShouldReturnFalse_ForDifferentHourTimeRanges()
    {
        // Arrange
        var range1 = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent);
        object range2 = new HourTimeRange(new TimeSpan(11, 0, 0), new TimeSpan(13, 0, 0), 2, DailyHourFrequency.Recurrent);

        // Act & Assert
        range1.Equals(range2).Should().BeFalse();
    }

    [Fact]
    public void EqualsObject_ShouldReturnFalse_WhenComparedWithNull()
    {
        // Arrange
        var range = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent);

        // Act & Assert
        range.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void EqualsObject_ShouldReturnFalse_WhenComparedWithDifferentType()
    {
        // Arrange
        var range = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0), 1, DailyHourFrequency.Recurrent);
        var differentTypeObject = "This is a string";

        // Act & Assert
        range.Equals(differentTypeObject).Should().BeFalse();
    }
}