using SchedulerApplication.Models;

namespace SchedulerApplication.Interfaces;

// Centralizes the logic for generating execution times, making the code easier to understand and maintain.
// Allows adding new strategies for generating execution times.
    public interface IExecutionTimeGenerator
    {
        List<DateTime> GenerateExecutions(SchedulerConfiguration configuration);
    }




