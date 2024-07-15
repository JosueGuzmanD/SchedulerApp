﻿using SchedulerApplication.Interfaces;
using SchedulerApplication.Models.FrequencyConfigurations;
using SchedulerApplication.Models;

namespace SchedulerApplication.Services.DateCalculator;

    public class DailyDateCalculator : IDateCalculator
    {
        public List<DateTime> CalculateDates(SchedulerConfiguration config)
        {
            var dailyConfig = (DailyFrequencyConfiguration)config;
            var results = new List<DateTime>();

            var currentDate = dailyConfig.CurrentDate.Date;

            while (results.Count < 12 && currentDate <= dailyConfig.Limits.LimitEndDateTime)
            {
                if (currentDate >= dailyConfig.Limits.LimitStartDateTime)
                {
                    results.Add(currentDate);
                }
                currentDate = currentDate.AddDays(1);
            }

            return results;
        }
    }

