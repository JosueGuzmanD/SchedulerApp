using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models.ValueObjects;

    public class DailyExecutionCalculator : IDailyExecutionCalculator
    {
        public IEnumerable<DateTime> CalculateDailyExecutions(DailyFrequencyConfiguration config)
        {
            var results = new List<DateTime>();
            var currentDate = config.CurrentDate;
            var startTime = config.HourTimeRange.StartHour;
            var endTime = config.HourTimeRange.EndHour;
            var interval = config.HourlyInterval;
            var limits = config.Limits;

            while (currentDate <= limits.LimitEndDateTime && results.Count < 12)
            {
                var currentHour = currentDate.Date.Add(startTime);

                if (startTime <= endTime)
                {
                    results.AddRange(GenerateHourlyExecutions(currentHour, endTime, interval, limits));
                }
                else
                {
                    results.AddRange(GenerateHourlyExecutionsCrossingMidnight(currentHour, endTime, interval, limits));
                }

                currentDate = currentDate.AddDays(1);
            }

            return results;
        }

        private IEnumerable<DateTime> GenerateHourlyExecutions(DateTime currentHour, TimeSpan endTime, int interval, LimitsTimeInterval limits)
        {
            var results = new List<DateTime>();
            while (currentHour.TimeOfDay <= endTime && results.Count < 12 && currentHour <= limits.LimitEndDateTime)
            {
                if (currentHour >= limits.LimitStartDateTime && currentHour <= limits.LimitEndDateTime)
                {
                    results.Add(currentHour);
                }
                currentHour = currentHour.AddHours(interval);
            }

            return results;
        }

        private IEnumerable<DateTime> GenerateHourlyExecutionsCrossingMidnight(DateTime currentHour, TimeSpan endTime, int interval, LimitsTimeInterval limits)
        {
            var results = new List<DateTime>();

            while (currentHour.TimeOfDay < TimeSpan.FromHours(24) && results.Count < 12 && currentHour <= limits.LimitEndDateTime)
            {
                if (currentHour >= limits.LimitStartDateTime && currentHour <= limits.LimitEndDateTime)
                {
                    results.Add(currentHour);
                }
                currentHour = currentHour.AddHours(interval);
            }

            currentHour = currentHour.Date.AddDays(1).Date.Add(endTime);

            while (currentHour.TimeOfDay <= endTime && results.Count < 12 && currentHour <= limits.LimitEndDateTime)
            {
                if (currentHour >= limits.LimitStartDateTime && currentHour <= limits.LimitEndDateTime)
                {
                    results.Add(currentHour);
                }
                currentHour = currentHour.AddHours(interval);
            }

            return results;
        }
    }
