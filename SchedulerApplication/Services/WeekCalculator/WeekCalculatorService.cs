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
            foreach (var day in daysOfWeek)
            {
                var nextDate = currentDate.Date;
                if (nextDate.DayOfWeek > day)
                {
                    nextDate = nextDate.AddDays(7 - (nextDate.DayOfWeek - day));
                }
                else
                {
                    nextDate = nextDate.AddDays(day - nextDate.DayOfWeek);
                }

                if (!dates.Contains(nextDate))
                {
                    dates.Add(nextDate);
                }
            }

            currentDate = currentDate.AddDays(7 * weekInterval);
            iterations++;
        }

        return dates;
    }
}

