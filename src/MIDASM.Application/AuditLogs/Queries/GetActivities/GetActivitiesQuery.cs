
using MIDASM.Application.Commons.Models.Auditlogs;
using MIDASM.Application.Dispatcher.Queries;
using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.AuditLogs.Queries.GetActivities;

public class GetActivitiesQuery : IQuery<Result<PaginationResult<AuditLogResponse>>>
{
    public AuditLogQueryParameters AuditLogQueryParameters { get; set; } = default!;
}
