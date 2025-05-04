
namespace MIDASM.Application.Commons.Models.Auditlogs;

public class UserActiveDaysAuditLog
{
    public Guid UserId { get; set; }
    public int ActiveDays { get; set; }
}
