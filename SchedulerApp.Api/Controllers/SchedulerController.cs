using Microsoft.AspNetCore.Mvc;
using SchedulerApp.Application.Dtos;
using SchedulerApp.Application.Services;
using SchedulerApp.Domain.Entities;

namespace SchedulerApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulerController : ControllerBase
    {
        private readonly ScheduleTypeSelector _scheduleTypeSelector;

        public SchedulerController(ScheduleTypeSelector scheduleTypeSelector)
        {
            _scheduleTypeSelector = scheduleTypeSelector;
        }

        [HttpPost]
        public ActionResult<ScheduleOutput> createSchedule([FromBody] SchedulerConfiguration configuration)
        {
          var scheduler = _scheduleTypeSelector.GetScheduleType(configuration.Type);
          var result =  scheduler.getNextExecutionTime(configuration);

            return Ok(result);
        }
    }
}
