
namespace MIDASM.Application.Commons.Models.SchedulerJobs;

public class TriggerJobDeleteRequest
{
    public string JobName { get; set; } = default!;
    public string? JobGroup { get; set; } = default;
    public string TriggerName { get; set; } = default!;
    public string? TriggerGroup { get; set; } = default;
}
