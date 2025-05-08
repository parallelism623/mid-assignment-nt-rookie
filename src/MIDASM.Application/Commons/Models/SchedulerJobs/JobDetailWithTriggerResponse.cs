
namespace MIDASM.Application.Commons.Models.SchedulerJobs;

public class JobDetailWithTriggerResponse
{
    public string JobName { get; set; } = default!;
    public string JobGroup { get; set; } = default!;
    public string? Description { get; set; } = default;
    public bool Durable { get; set; }
    public bool NonConcurrent { get; set; }
    public bool RequestsRecovery { get; set; }
    public string? JobData { get; set; }
    public List<TriggerJobResponse> TriggerJobs { get; set; } = new();
}


public class TriggerJobResponse
{
    public string TriggerName { get; set; }
    public string TriggerGroup { get; set; }
    public string? Description { get; set; } = default;
    public string? TimeZoneID { get; set; } = default;
    public string? CronExpression { get; set; } = default;
}
