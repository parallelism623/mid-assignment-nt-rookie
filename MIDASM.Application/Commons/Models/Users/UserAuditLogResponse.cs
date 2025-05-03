
namespace MIDASM.Application.Commons.Models.Users;

public class UserAuditLogResponse
{
    public Guid Id { get; set; }
    public string? EntityName { get; set; } = default!;
    public string? EntityId { get; set; } = default!;
    public DateTime TimeStamp { get; set; }
    public string? Description { get; set; }
}
