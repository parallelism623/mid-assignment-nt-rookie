
namespace MIDASM.Application.Commons.Models.Users;

public class UserAuditLogQueryParameters : QueryParameters
{
    public string EntityName { get; set; } = default!;
}
