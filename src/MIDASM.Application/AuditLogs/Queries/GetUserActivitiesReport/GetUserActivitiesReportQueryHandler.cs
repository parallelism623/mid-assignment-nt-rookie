
using MIDASM.Application.Commons.Models.Auditlogs;
using MIDASM.Application.Dispatcher.Queries;
using MIDASM.Domain.Repositories;

namespace MIDASM.Application.AuditLogs.Queries.GetUserActivitiesReport;

public class GetUserActivitiesReportQueryHandler
(IAuditLoggerRepository auditLoggerRepository)
    : IQueryHandler<GetUserActivitiesReportQuery, List<UserActiveDaysAuditLog>>
{
    public async Task<List<UserActiveDaysAuditLog>> HandlerAsync(GetUserActivitiesReportQuery request, CancellationToken cancellationToken = default)
    {
        var userActivitiesQueryParameters = request.QueryParameters;
        var queryData = auditLoggerRepository.GetAuditLogQueryable().Where(ad =>
                userActivitiesQueryParameters.UserIds.Contains(ad.UserId)
                && DateOnly.FromDateTime(ad.TimeStamp.Date) <= userActivitiesQueryParameters.ToDate
                && DateOnly.FromDateTime(ad.TimeStamp.Date) >= userActivitiesQueryParameters.FromDate)
            .GroupBy(ad => new { ad.UserId, })
            .Select(g => new UserActiveDaysAuditLog
            {
                UserId = g.Key.UserId, ActiveDays = g.Select(ad => ad.TimeStamp.Date).Distinct().Count()
            });
        return await auditLoggerRepository.ToListAsync(queryData);
    }
}
