
namespace MIDASM.Application.Commons.Models.SchedulerJobs;

public class CronTriggerCreateRequest
{
    public string JobName { get; set; } = default!;
    public string JobGroup { get; set; } = default!;
    public string JobType { get; set; } = default!;
    public string TriggerName { get; set; } = default!;
    public string TriggerGroup { get; set; } = default!;
    public string CronExpression { get; set; } = default!;
    public DateTimeOffset? StartAt { get; set; }  
    public DateTimeOffset? EndAt { get; set; }

    public string? TimeZone { get; set; } = default;
}
