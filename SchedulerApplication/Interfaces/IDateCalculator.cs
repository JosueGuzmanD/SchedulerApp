using SchedulerApplication.Models;

namespace SchedulerApplication.Interfaces;

// Allows separating date calculation logic, making it easier to understand and maintain.
// Allows adding new types of date calculations (daily, weekly, monthly) without affecting existing code.
public interface IDateCalculator
{
    List<DateTime> CalculateDates(SchedulerConfiguration config, int maxExecutions);
}

