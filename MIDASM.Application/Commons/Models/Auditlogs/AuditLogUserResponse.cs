namespace MIDASM.Application.Commons.Models.Auditlogs;

public class AuditLogResponse
{
    public Guid Id { get; set; }
    public string? EntityName { get; set; } = default!;
    public string? EntityId { get; set; } = default!;
    public DateTime TimeStamp { get; set; }
    public Guid UserId { get; set; }
    public string? Username { get; set; }
    public string? Description { get; set; }

    public List<AuditLogDataResponse>? AuditLogDatas { get; set; }
}
public class AuditLogDataResponse
{
    public Guid Id { get; set; }
    public string? NewValue { get; set; }
    public string? OriginalValue { get; set; }
    public string? PropertyName { get; set; }
}
