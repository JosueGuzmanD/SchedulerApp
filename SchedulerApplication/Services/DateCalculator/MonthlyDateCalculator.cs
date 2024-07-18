using SchedulerApplication.Interfaces;
using SchedulerApplication.Models;

namespace SchedulerApplication.Services.DateCalculator;

public class MonthlyDateCalculator : IDateCalculator
{
    public List<DateTime> CalculateDates(SchedulerConfiguration config)
    {
        return calculateEveryDateTimes()

            //Every Flow

        }

    private DateTime CalculateEveryDateTimes(int days, int MonthsInterval, DateTime startingDate)
    {

        if (startingDate.Day < days)
        {

            return startingDate.a


            }



    }


}

