namespace SchedulerApplication.ValueObjects;

public class HourTimeRange : IEquatable<HourTimeRange>
{
    public TimeSpan StartHour { get; }
    public TimeSpan EndHour { get; }

    // Constructor solo para el caso 'Once'
    public HourTimeRange(TimeSpan startHour)
    {
        StartHour = startHour;
        EndHour = TimeSpan.Zero;
    }

    public HourTimeRange(TimeSpan startHour, TimeSpan endHour)
    {
        StartHour = startHour;
        EndHour = endHour;
    }

    public bool Equals(HourTimeRange other)
    {
        if (other == null) return false;
        return StartHour == other.StartHour && EndHour == other.EndHour;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as HourTimeRange);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StartHour, EndHour);
    }

    public static bool operator ==(HourTimeRange left, HourTimeRange right)
    {
        return EqualityComparer<HourTimeRange>.Default.Equals(left, right);
    }

    public static bool operator !=(HourTimeRange left, HourTimeRange right)
    {
        return !(left == right);
    }
}

