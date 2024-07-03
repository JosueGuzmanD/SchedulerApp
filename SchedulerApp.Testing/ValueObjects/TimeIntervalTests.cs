using FluentAssertions;
using SchedulerApplication.ValueObjects;

namespace SchedulerApp.Testing.ValueObjects;

public class TimeIntervalTests
{
    [Fact]
    public void Constructor_ShouldThrowException_WhenLimitEndDateTimeIsLessThanLimitStartDateTime()
    {
        // Arrange
        var startDateTime = new DateTime(2024, 1, 1, 10, 0, 0);
        var endDateTime = new DateTime(2023, 1, 1, 10, 0, 0);

        // Act
        Action act = () => new TimeInterval(startDateTime, endDateTime);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("End date must be greater than or equal to start date");
    }

    [Fact]
    public void Constructor_ShouldInitializeProperties_WhenParametersAreValid()
    {
        // Arrange
        var startDateTime = new DateTime(2024, 1, 1, 10, 0, 0);
        var endDateTime = new DateTime(2024, 1, 1, 12, 0, 0);

        // Act
        var interval = new TimeInterval(startDateTime, endDateTime);

        // Assert
        interval.LimitStartDateTime.Should().Be(startDateTime);
        interval.LimitEndDateTime.Should().Be(endDateTime);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForEquivalentTimeIntervals()
    {
        // Arrange
        var startDateTime = new DateTime(2024, 1, 1, 10, 0, 0);
        var endDateTime = new DateTime(2024, 1, 1, 12, 0, 0);

        var interval1 = new TimeInterval(startDateTime, endDateTime);
        var interval2 = new TimeInterval(startDateTime, endDateTime);

        // Act & Assert
        interval1.Equals(interval2).Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_ForDifferentTimeIntervals()
    {
        // Arrange
        var startDateTime1 = new DateTime(2024, 1, 1, 10, 0, 0);
        var endDateTime1 = new DateTime(2024, 1, 1, 12, 0, 0);
        var startDateTime2 = new DateTime(2024, 1, 1, 10, 0, 0);
        var endDateTime2 = new DateTime(2024, 1, 1, 13, 0, 0);

        var interval1 = new TimeInterval(startDateTime1, endDateTime1);
        var interval2 = new TimeInterval(startDateTime2, endDateTime2);

        // Act & Assert
        interval1.Equals(interval2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameHashCode_ForEquivalentTimeIntervals()
    {
        // Arrange
        var startDateTime = new DateTime(2024, 1, 1, 10, 0, 0);
        var endDateTime = new DateTime(2024, 1, 1, 12, 0, 0);

        var interval1 = new TimeInterval(startDateTime, endDateTime);
        var interval2 = new TimeInterval(startDateTime, endDateTime);

        // Act & Assert
        interval1.GetHashCode().Should().Be(interval2.GetHashCode());
    }

    [Fact]
    public void OperatorEquals_ShouldReturnTrue_ForEquivalentTimeIntervals()
    {
        // Arrange
        var startDateTime = new DateTime(2024, 1, 1, 10, 0, 0);
        var endDateTime = new DateTime(2024, 1, 1, 12, 0, 0);

        var interval1 = new TimeInterval(startDateTime, endDateTime);
        var interval2 = new TimeInterval(startDateTime, endDateTime);

        // Act & Assert
        (interval1 == interval2).Should().BeTrue();
    }

    [Fact]
    public void OperatorNotEquals_ShouldReturnTrue_ForDifferentTimeIntervals()
    {
        // Arrange
        var startDateTime1 = new DateTime(2024, 1, 1, 10, 0, 0);
        var endDateTime1 = new DateTime(2024, 1, 1, 12, 0, 0);
        var startDateTime2 = new DateTime(2024, 1, 1, 10, 0, 0);
        var endDateTime2 = new DateTime(2024, 1, 1, 13, 0, 0);

        var interval1 = new TimeInterval(startDateTime1, endDateTime1);
        var interval2 = new TimeInterval(startDateTime2, endDateTime2);

        // Act & Assert
        (interval1 != interval2).Should().BeTrue();
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var startDateTime = new DateTime(2024, 1, 1, 10, 0, 0);
        var endDateTime = new DateTime(2024, 1, 1, 12, 0, 0);
        var interval = new TimeInterval(startDateTime, endDateTime);

        // Act
        var result = interval.ToString();

        // Assert
        result.Should().Be($"{startDateTime}- {endDateTime}");
    }

    [Fact]
    public void EqualsObject_ShouldReturnTrue_ForEquivalentTimeIntervals()
    {
        // Arrange
        var startDateTime = new DateTime(2024, 1, 1, 10, 0, 0);
        var endDateTime = new DateTime(2024, 1, 1, 12, 0, 0);

        var interval1 = new TimeInterval(startDateTime, endDateTime);
        object interval2 = new TimeInterval(startDateTime, endDateTime);

        // Act & Assert
        interval1.Equals(interval2).Should().BeTrue();
    }

    [Fact]
    public void EqualsObject_ShouldReturnFalse_ForDifferentTimeIntervals()
    {
        // Arrange
        var startDateTime1 = new DateTime(2024, 1, 1, 10, 0, 0);
        var endDateTime1 = new DateTime(2024, 1, 1, 12, 0, 0);
        var startDateTime2 = new DateTime(2024, 1, 1, 10, 0, 0);
        var endDateTime2 = new DateTime(2024, 1, 1, 13, 0, 0);

        var interval1 = new TimeInterval(startDateTime1, endDateTime1);
        object interval2 = new TimeInterval(startDateTime2, endDateTime2);

        // Act & Assert
        interval1.Equals(interval2).Should().BeFalse();
    }

    [Fact]
    public void EqualsObject_ShouldReturnFalse_WhenComparedWithNull()
    {
        // Arrange
        var interval = new TimeInterval(new DateTime(2024, 1, 1, 10, 0, 0), new DateTime(2024, 1, 1, 12, 0, 0));

        // Act & Assert
        interval.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void EqualsObject_ShouldReturnFalse_WhenComparedWithDifferentType()
    {
        // Arrange
        var interval = new TimeInterval(new DateTime(2024, 1, 1, 10, 0, 0), new DateTime(2024, 1, 1, 12, 0, 0));
        var differentTypeObject = "This is a string";

        // Act & Assert
        interval.Equals(differentTypeObject).Should().BeFalse();
    }
    [Fact]
    public void EqualsTimeInterval_ShouldReturnFalse_ForDifferentLimitStartDateTime()
    {
        // Arrange
        var startDateTime1 = new DateTime(2024, 1, 1, 10, 0, 0);
        var endDateTime1 = new DateTime(2024, 1, 2, 10, 0, 0); 
        var startDateTime2 = new DateTime(2024, 1, 3, 10, 0, 0); 
        var endDateTime2 = new DateTime(2024, 1, 4, 10, 0, 0); 

        var interval1 = new TimeInterval(startDateTime1, endDateTime1);
        var interval2 = new TimeInterval(startDateTime2, endDateTime2);

        // Act & Assert
        interval1.Equals(interval2).Should().BeFalse();
    }

    [Fact]
    public void EqualsTimeInterval_ShouldReturnFalse_ForDifferentLimitEndDateTime()
    {
        // Arrange
        var startDateTime = new DateTime(2024, 1, 1, 10, 0, 0);
        var endDateTime1 = new DateTime(2024, 1, 1, 12, 0, 0);
        var endDateTime2 = new DateTime(2024, 1, 1, 13, 0, 0);

        var interval1 = new TimeInterval(startDateTime, endDateTime1);
        var interval2 = new TimeInterval(startDateTime, endDateTime2);

        // Act & Assert
        interval1.Equals(interval2).Should().BeFalse();
    }

    [Fact]
    public void EqualsTimeInterval_ShouldReturnTrue_ForSameLimitStartDateTimeAndNullLimitEndDateTime()
    {
        // Arrange
        var startDateTime = new DateTime(2024, 1, 1, 10, 0, 0);

        var interval1 = new TimeInterval(startDateTime);
        var interval2 = new TimeInterval(startDateTime);

        // Act & Assert
        interval1.Equals(interval2).Should().BeTrue();
    }

    [Fact]
    public void EqualsTimeInterval_ShouldReturnFalse_ForSameLimitStartDateTimeAndDifferentLimitEndDateTime()
    {
        // Arrange
        var startDateTime = new DateTime(2024, 1, 1, 10, 0, 0);

        var interval1 = new TimeInterval(startDateTime, new DateTime(2024, 1, 1, 12, 0, 0));
        var interval2 = new TimeInterval(startDateTime, new DateTime(2024, 1, 1, 13, 0, 0));

        // Act & Assert
        interval1.Equals(interval2).Should().BeFalse();
    }
}
