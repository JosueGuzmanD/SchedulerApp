using SchedulerApplication.Services.Interfaces;

namespace SchedulerApplication.Services.WeekConfigurator;

    public class WeekCalculatorService: IWeekCalculatorService
    {

    public List<DateTime> CalculateWeeklyDates(DateTime initialDate, List<DayOfWeek> daysOfWeek, int weekInterval)
    {
        List<DateTime> dates = new List<DateTime>();
        DateTime currentDate = initialDate;
        int iterations = 0;

        while (iterations < 3)
        {
            foreach (var day in daysOfWeek)
            {
                DateTime nextDate = currentDate.Date;
                if (nextDate.DayOfWeek <= day)
                {
                    nextDate = nextDate.AddDays(day - nextDate.DayOfWeek);
                }
                else
                {
                    nextDate = nextDate.AddDays(7 - (nextDate.DayOfWeek - day));
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

