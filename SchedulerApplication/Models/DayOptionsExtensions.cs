using SchedulerApplication.Common.Enums;

namespace SchedulerApplication.Models;

public static class WeekOptionsExtensions
{
    public static IEnumerable<DayOfWeek> GetDays(this WeekOptions weekOption)
    {
        return weekOption switch
        {
            WeekOptions.Monday or WeekOptions.Tuesday or WeekOptions.Wednesday or WeekOptions.Thursday or WeekOptions.Friday
                or WeekOptions.Saturday or WeekOptions.Sunday => new List<DayOfWeek> { (DayOfWeek)weekOption },
            WeekOptions.Weekday => new List<DayOfWeek>
            {
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday
            },
            WeekOptions.WeekendDay => new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday },
            WeekOptions.AnyDay => Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>(),
            _ => throw new ArgumentOutOfRangeException(nameof(weekOption), weekOption, null)
        };
    }

    public static string ToCustomString(this WeekOptions weekOption)
    {
        return weekOption switch
        {
            WeekOptions.Weekday => "Weekday",
            WeekOptions.WeekendDay => "WeekendDay",
            WeekOptions.AnyDay => "AnyDay",
            _ => weekOption.ToString()
        };
    }
}

