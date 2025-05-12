
using MIDASM.Application.Commons.Models.Users;
using MIDASM.Application.Dispatcher.Queries;
using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.AuditLogs.Queries.GetUserActivities;

public class GetUserActivitiesQuery : IQuery<Result<PaginationResult<UserAuditLogResponse>>>
{
    public Guid UserId { get; set; }
    public UserAuditLogQueryParameters QueryParameters { get; set; } = default!;
}
