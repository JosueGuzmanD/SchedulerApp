using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.WeekCalculator;

public class WeekCalculatorService : IWeekCalculatorService
{
    public List<DateTime> CalculateWeeklyDates(DateTime initialDate, List<DayOfWeek> daysOfWeek, int weekInterval)
    {
        var dates = new List<DateTime>();
        var currentDate = initialDate;
        var iterations = 0;

        while (iterations < 3)
        {
            var weeklyDates = CalculateDatesForCurrentWeek(currentDate, daysOfWeek);
            dates.AddRange(weeklyDates);
            currentDate = currentDate.AddDays(7 * weekInterval);
            iterations++;
        }

        return dates;
    }

    private List<DateTime> CalculateDatesForCurrentWeek(DateTime currentDate, List<DayOfWeek> daysOfWeek)
    {
        var weeklyDates = new List<DateTime>();
        foreach (var day in daysOfWeek)
        {
            var nextDate = GetNextSpecificDay(currentDate, day);
            if (!weeklyDates.Contains(nextDate))
            {
                weeklyDates.Add(nextDate);
            }
        }
        return weeklyDates;
    }

    private DateTime GetNextSpecificDay(DateTime currentDate, DayOfWeek day)
    {
        int daysUntilNext = ((int)day - (int)currentDate.DayOfWeek + 7) % 7;
        return currentDate.AddDays(daysUntilNext == 0 ? 7 : daysUntilNext);
    }
}


