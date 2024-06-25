
namespace SchedulerApp.Application.Dtos;

public class ScheduleOutputDto
{
    public string Description { get; set; }
    public List<DateTime> ExecutionTime { get; set; } = new List<DateTime>();
}