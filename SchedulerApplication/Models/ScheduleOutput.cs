using System.Collections;
namespace SchedulerApp.Domain.Entities;

    public class ScheduleOutput
    {
        public string Description { get; set; }
        public List<DateTime> ExecutionTime { get; set; }= new List<DateTime>();
    }

