namespace SchedulerApplication.ValueObjects;

public class TimeInterval : IEquatable<TimeInterval>
{
    public DateTime LimitStartDateTime { get; }
    public DateTime? LimitEndDateTime { get; }

    public TimeInterval(DateTime limitStartDateTime, DateTime limitEndDateTime)
    {
        if (limitEndDateTime < limitStartDateTime)
            throw new ArgumentException("End date must be greater than or equal to start date");

        LimitStartDateTime = limitStartDateTime;
        LimitEndDateTime = limitEndDateTime;
    }

    public bool Equals(TimeInterval? other)
    {
        if (other is null) return false;
        return LimitStartDateTime == other.LimitStartDateTime && LimitEndDateTime == other.LimitEndDateTime;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as TimeInterval);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LimitStartDateTime, LimitEndDateTime);
    }

    public static bool operator ==(TimeInterval left, TimeInterval right)
    {
        return EqualityComparer<TimeInterval>.Default.Equals(left, right);
    }

    public static bool operator !=(TimeInterval left, TimeInterval right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"{LimitStartDateTime}- {LimitEndDateTime}";
    }
}
