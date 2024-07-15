using SchedulerApplication.Models;

namespace SchedulerApplication.Interfaces;

    public interface IExecutionTimeGenerator
    {
        List<DateTime> GenerateExecutions(SchedulerConfiguration configuration);
    }




