using SchedulerApp.Domain.Common.Enums;
using SchedulerApp.Domain.Interfaces;

namespace SchedulerApplication.Services.Implementations;

public class ScheduleTypeSelector
{
    public IScheduleType GetScheduleType(SchedulerType type)
    {

        switch (type)
        {
            case SchedulerType.Once: return new ScheduleTypeOnce();
            case SchedulerType.Recurring: return new ScheduleTypeRecurring();
            default: return null;
        }

    }
}

