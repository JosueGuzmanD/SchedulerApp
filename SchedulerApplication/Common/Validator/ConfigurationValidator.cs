﻿using SchedulerApplication.Models;

namespace SchedulerApplication.Common.Validator;

public class ConfigurationValidator 
{
    public void Validate(SchedulerConfiguration configuration)
    {
        if (!configuration.IsEnabled)
        {
            throw new ArgumentException("Configuration must be enabled.");
        }
    }
}
