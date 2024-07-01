namespace SchedulerApplication.Services.Interfaces;

    public interface IWeekCalculatorService
    {
        List<DateTime> CalculateWeeklyDates(DateTime initialDate, List<DayOfWeek> daysOfWeek, int weekInterval);
    }

