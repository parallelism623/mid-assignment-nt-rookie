
using MIDASM.Application.Commons.Models.Users;
using MIDASM.Application.Dispatcher.Queries;
using MIDASM.Domain.Repositories;
using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.AuditLogs.Queries.GetUserActivities;

public class GetUserActivitiesQueryHandler(IAuditLoggerRepository auditLoggerRepository) 
    : IQueryHandler<GetUserActivitiesQuery, Result<PaginationResult<UserAuditLogResponse>>>
{
    public async Task<Result<PaginationResult<UserAuditLogResponse>>> HandlerAsync(GetUserActivitiesQuery request, CancellationToken cancellationToken = default)
    {
        var queryParameters = request.QueryParameters;
        var userId = request.UserId;
        var query = auditLoggerRepository.GetAuditLogQueryable();

        query = query.Where(al => al.UserId == userId && al.EntityName == queryParameters.EntityName)
            .OrderByDescending(al => al.TimeStamp);
        var totalCount = await auditLoggerRepository.CountAsync(query);

        var dataQuery = query.Skip(queryParameters.PageSize * (queryParameters.PageIndex - 1))
            .Take(queryParameters.PageSize)
            .Select(al => new UserAuditLogResponse
            {
                Id = al.Id,
                EntityId = al.EntityId,
                EntityName = al.EntityName,
                TimeStamp = al.TimeStamp,
                Description = al.Description
            });

        var data = await auditLoggerRepository.ToListAsync(dataQuery);

        return PaginationResult<UserAuditLogResponse>.Create(totalCount, data);
    }
}
