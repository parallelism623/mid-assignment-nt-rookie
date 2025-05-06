
namespace MIDASM.Application.Commons.Models.Auditlogs;

public class UserActivitiesQueryParameters
{
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public List<Guid> UserIds { get; set; } = new List<Guid>();
}
