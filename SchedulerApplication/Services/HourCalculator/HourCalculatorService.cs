using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.ValueObjects;

namespace SchedulerApplication.Services.HourCalculator;

    public class HourCalculatorService : IHourCalculator
    {
        public List<DateTime> CalculateHours(List<DateTime> dates, HourTimeRange hourTimeRange, int hourlyInterval)
        {
            var results = new List<DateTime>();

            foreach (var date in dates)
            {
                var currentHour = date.Date.Add(hourTimeRange.StartHour);

                while (currentHour.TimeOfDay <= hourTimeRange.EndHour && results.Count < 12)
                {
                    results.Add(currentHour);
                    currentHour = currentHour.AddHours(hourlyInterval);
                }
            }

            return results;
        }
    }

