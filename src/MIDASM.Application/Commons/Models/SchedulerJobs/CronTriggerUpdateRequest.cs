
namespace MIDASM.Application.Commons.Models.SchedulerJobs;

public class CronTriggerUpdateRequest
{
    public string TriggerName { get; set; } = default!;
    public string TriggerGroup { get; set; } = default!;
    public string CronExpression { get; set; } = default!;
    public DateTimeOffset? StartAt { get; set; }
    public DateTimeOffset? EndAt { get; set; }
}
