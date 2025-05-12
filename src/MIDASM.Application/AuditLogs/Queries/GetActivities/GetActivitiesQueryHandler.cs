
using MIDASM.Application.Commons.Models;
using MIDASM.Application.Commons.Models.Auditlogs;
using MIDASM.Application.Dispatcher.Queries;
using MIDASM.Domain.Repositories;
using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.AuditLogs.Queries.GetActivities;

public class GetActivitiesQueryHandler(
    IAuditLoggerRepository auditLoggerRepository)
    : IQueryHandler<GetActivitiesQuery, Result<PaginationResult<AuditLogResponse>>>
{
    public async Task<Result<PaginationResult<AuditLogResponse>>> HandlerAsync(GetActivitiesQuery request, CancellationToken cancellationToken = default)
    {
        var queryParameters = request.AuditLogQueryParameters;
        var query = auditLoggerRepository.GetAuditLogQueryable();
        var queryAuditLogData = auditLoggerRepository.GetAuditLogDataQueryable();
        
        if(!string.IsNullOrEmpty(queryParameters.ServiceName))
        {
            query = query.Where(al => (!string.IsNullOrEmpty(al.ServiceName) && al.ServiceName.Contains(queryParameters.ServiceName)));
        }

        query = query.OrderByDescending(al => al.TimeStamp);

        var totalCount = await auditLoggerRepository.CountAsync(query);

        var dataQuery = query.Skip(queryParameters.Skip)
            .Take(queryParameters.Take)
            .GroupJoin(
                queryAuditLogData,
                al => al.Id,
                ald => ald.AuditLogId,
                (al, ald) => new AuditLogResponse
                {
                    Id = al.Id,
                    EntityId = al.EntityId,
                    EntityName = al.EntityName,
                    TimeStamp = al.TimeStamp,
                    Description = al.Description,
                    UserId = al.UserId,
                    Username = al.Username,
                    AuditLogDatas = ald.Select(l => new AuditLogDataResponse
                    {
                        Id = l.Id,
                        PropertyName = l.PropertyName,
                        NewValue = l.NewValue,
                        OriginalValue = l.OriginalValue
                    }).ToList()
                });

        var data = await auditLoggerRepository.ToListAsync(dataQuery);

        return PaginationResult<AuditLogResponse>.Create(totalCount, data);
    }
    
}
