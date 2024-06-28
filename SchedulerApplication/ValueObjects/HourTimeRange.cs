using SchedulerApplication.Common.Enums;

namespace SchedulerApplication.ValueObjects;

    public class HourTimeRange : IEquatable<HourTimeRange>
    {
    public TimeSpan StartHour { get; }
    public TimeSpan EndHour { get; }
    public int HourlyInterval { get; }
    public DailyHourFrequency HourlyFrequency { get; }

    public HourTimeRange(TimeSpan startHour, TimeSpan endHour, int hourlyInterval, DailyHourFrequency hourlyFrequency)
    {
        if (startHour > endHour)
        {
            throw new ArgumentException("StartHour must be less than or equal to EndHour.");
        }

        if (hourlyInterval <= 0)
        {
            throw new ArgumentException("HourlyInterval must be greater than 0.");
        }

        StartHour = startHour;
        EndHour = endHour;
        HourlyInterval = hourlyInterval;
        HourlyFrequency = hourlyFrequency;
    }

    public bool Equals(HourTimeRange other)
    {
        if (other == null) return false;
        return StartHour == other.StartHour && EndHour == other.EndHour && HourlyInterval == other.HourlyInterval && HourlyFrequency == other.HourlyFrequency;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as HourTimeRange);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StartHour, EndHour, HourlyInterval, HourlyFrequency);
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

