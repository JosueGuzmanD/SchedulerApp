using FluentAssertions;
using SchedulerApplication.ValueObjects;

namespace SchedulerApp.Testing.ValueObjects;

public class HourTimeRangeTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties_ForOnceCase()
    {
        // Arrange
        var startHour = new TimeSpan(10, 0, 0);

        // Act
        var range = new HourTimeRange(startHour);

        // Assert
        range.StartHour.Should().Be(startHour);
        range.EndHour.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void Constructor_ShouldInitializeProperties_ForRecurrentCase()
    {
        // Arrange
        var startHour = new TimeSpan(10, 0, 0);
        var endHour = new TimeSpan(12, 0, 0);

        // Act
        var range = new HourTimeRange(startHour, endHour);

        // Assert
        range.StartHour.Should().Be(startHour);
        range.EndHour.Should().Be(endHour);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForEquivalentHourTimeRanges()
    {
        // Arrange
        var startHour = new TimeSpan(10, 0, 0);
        var endHour = new TimeSpan(12, 0, 0);

        var range1 = new HourTimeRange(startHour, endHour);
        var range2 = new HourTimeRange(startHour, endHour);

        // Act & Assert
        range1.Equals(range2).Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_ForDifferentHourTimeRanges()
    {
        // Arrange
        var range1 = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0));
        var range2 = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0));

        // Act & Assert
        range1.Equals(range2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameHashCode_ForEquivalentHourTimeRanges()
    {
        // Arrange
        var startHour = new TimeSpan(10, 0, 0);
        var endHour = new TimeSpan(12, 0, 0);

        var range1 = new HourTimeRange(startHour, endHour);
        var range2 = new HourTimeRange(startHour, endHour);

        // Act & Assert
        range1.GetHashCode().Should().Be(range2.GetHashCode());
    }

    [Fact]
    public void OperatorEquals_ShouldReturnTrue_ForEquivalentHourTimeRanges()
    {
        // Arrange
        var startHour = new TimeSpan(10, 0, 0);
        var endHour = new TimeSpan(12, 0, 0);

        var range1 = new HourTimeRange(startHour, endHour);
        var range2 = new HourTimeRange(startHour, endHour);

        // Act & Assert
        (range1 == range2).Should().BeTrue();
    }

    [Fact]
    public void OperatorNotEquals_ShouldReturnTrue_ForDifferentHourTimeRanges()
    {
        // Arrange
        var range1 = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0));
        var range2 = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0));

        // Act & Assert
        (range1 != range2).Should().BeTrue();
    }

    [Fact]
    public void EqualsObject_ShouldReturnTrue_ForEquivalentHourTimeRanges()
    {
        // Arrange
        var range1 = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0));
        object range2 = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0));

        // Act & Assert
        range1.Equals(range2).Should().BeTrue();
    }

    [Fact]
    public void EqualsObject_ShouldReturnFalse_ForDifferentHourTimeRanges()
    {
        // Arrange
        var range1 = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0));
        object range2 = new HourTimeRange(new TimeSpan(11, 0, 0), new TimeSpan(13, 0, 0));

        // Act & Assert
        range1.Equals(range2).Should().BeFalse();
    }

    [Fact]
    public void EqualsObject_ShouldReturnFalse_WhenComparedWithNull()
    {
        // Arrange
        var range = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0));

        // Act & Assert
        range.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void EqualsObject_ShouldReturnFalse_WhenComparedWithDifferentType()
    {
        // Arrange
        var range = new HourTimeRange(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0));
        var differentTypeObject = "This is a string";

        // Act & Assert
        range.Equals(differentTypeObject).Should().BeFalse();
    }
}
