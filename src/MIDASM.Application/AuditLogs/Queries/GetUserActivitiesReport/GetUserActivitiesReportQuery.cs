
using MIDASM.Application.Commons.Models.Auditlogs;
using MIDASM.Application.Dispatcher.Queries;

namespace MIDASM.Application.AuditLogs.Queries.GetUserActivitiesReport;

public class GetUserActivitiesReportQuery : IQuery<List<UserActiveDaysAuditLog>>
{
    public UserActivitiesQueryParameters QueryParameters { get; set; } = default!;
}
