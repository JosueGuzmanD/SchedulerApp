namespace SchedulerApplication.ValueObjects;

public class LimitsTimeInterval : IEquatable<LimitsTimeInterval>
{
    public DateTime LimitStartDateTime { get; set; }
    public DateTime? LimitEndDateTime { get; }

    public LimitsTimeInterval(DateTime limitStartDateTime)
    {
        LimitStartDateTime = limitStartDateTime;
        LimitEndDateTime = null;
    }

    public LimitsTimeInterval(DateTime limitStartDateTime, DateTime limitEndDateTime)
    {
        if (limitEndDateTime < limitStartDateTime)
            throw new ArgumentException("End date must be greater than or equal to start date");

        LimitStartDateTime = limitStartDateTime;
        LimitEndDateTime = limitEndDateTime;
    }

    public bool Equals(LimitsTimeInterval other)
    {
        if (other is null) return false;
        return LimitStartDateTime == other.LimitStartDateTime && LimitEndDateTime == other.LimitEndDateTime;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as LimitsTimeInterval);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LimitStartDateTime, LimitEndDateTime);
    }

    public static bool operator ==(LimitsTimeInterval left, LimitsTimeInterval right)
    {
        return EqualityComparer<LimitsTimeInterval>.Default.Equals(left, right);
    }

    public static bool operator !=(LimitsTimeInterval left, LimitsTimeInterval right)
    {
        return !(left == right);
    }

    public bool IsWithinInterval(DateTime date)
    {
        return date >= LimitStartDateTime && (LimitEndDateTime == null || date <= LimitEndDateTime);
    }
}
