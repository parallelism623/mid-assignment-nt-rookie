using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MIDASM.Application.Commons.Models.Auditlogs;
using MIDASM.Application.Commons.Models.Users;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.Services.Authentication;

using MIDASM.Contract.SharedKernel;
using MIDASM.Domain.Entities;
using MIDASM.Persistence.Specifications;
using System.Net;


namespace MIDASM.Persistence.UseCases;

public class AuditLogger(AuditLogDbContext auditDbContext,
    IExecutionContext executionContext,
    IHttpContextAccessor httpContextAccessor) : IAuditLogger
{

    public async Task<Result<PaginationResult<AuditLogResponse>>> GetActivitiesAsync(AuditLogQueryParameters queryParameters)
    {
        var query = auditDbContext.AuditLogs!.AsQueryable();
        var querySpecification = new AuditLogByQueryParametersSpecification(queryParameters);
        var queryAuditLogData = auditDbContext.AuditLogDatas!.AsQueryable();
        query = querySpecification.GetQuery(query);

        var totalCount = await query.CountAsync();

        var data = await query.Skip(queryParameters.Skip)
                        .Take(queryParameters.Take)
                        .GroupJoin(
                            queryAuditLogData, 
                            al => al.Id,
                            ald => ald.AuditLogId, 
                            (al, ald ) =>  new AuditLogResponse
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
                            })
                        .ToListAsync();

        return PaginationResult<AuditLogResponse>.Create(totalCount, data);
    }


    public async Task<Result<PaginationResult<UserAuditLogResponse>>> GetUserActivitiesAsync(Guid userId, UserAuditLogQueryParameters queryParameters)
    {
        var query = auditDbContext.AuditLogs!.AsQueryable();
        var querySpecification = new UserAuditLogByQueryParametersSpecification(userId, queryParameters);

        query = querySpecification.GetQuery(query);


        var totalCount = await query.CountAsync();

        var data = await query.Skip(queryParameters.PageSize * (queryParameters.PageIndex - 1))
                    .Take(queryParameters.PageSize)
                    .Select(al => new UserAuditLogResponse
                    {
                        Id = al.Id,
                        EntityId = al.EntityId,
                        EntityName = al.EntityName,
                        TimeStamp = al.TimeStamp,
                        Description = al.Description
                    })
                    .ToListAsync();

        return PaginationResult<UserAuditLogResponse>.Create(totalCount, data);
    }

    public async Task LogAsync(string entityId, string entityName, string desciption, Dictionary<string, (string?, string?)>? changedProperties = default)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var routeData = httpContext?.GetRouteData();
        var auditLog = new AuditLog()
        {
            Id = Guid.NewGuid(),
            UserId = executionContext.GetUserId(),
            BrowserInfo = httpContext?
                .Request
                .Headers[nameof(HttpRequestHeader.UserAgent)]
                .ToString(),
            HttpMethod = httpContext?.Request.Method,
            Username = executionContext.GetUserName(),
            Url = httpContext?.Request.Path.ToString(),
            ServiceName = routeData?.Values["controller"]?.ToString(),
            MethodName = routeData?.Values["action"]?.ToString()?.Replace("Async", ""),
            EntityId = entityId,
            EntityName = entityName,
            TimeStamp = DateTime.UtcNow,
            Description = desciption,
        };
        auditDbContext.Add(auditLog);
        var auditLogDatas = GetAuditLogDataFromChangedProperties(auditLog.Id, changedProperties);
        if (auditLogDatas.Count > 0)
        {
            auditDbContext.AddRange(auditLogDatas);
        }
        await auditDbContext.SaveChangesAsync();
    }


    private static List<AuditLogData> GetAuditLogDataFromChangedProperties(Guid auditLogId, Dictionary<string, (string?, string?)>? changedProperties)
    {
        var auditLogDatas = new List<AuditLogData>();
        if (changedProperties != null)
        {
            foreach (var it in changedProperties)
            {
                var auditLogData = new AuditLogData()
                {
                    Id = Guid.NewGuid(),
                    AuditLogId = auditLogId,
                    PropertyName = it.Key,
                    OriginalValue = it.Value.Item1,
                    NewValue = it.Value.Item2,
                };
                auditLogDatas.Add(auditLogData);
            }
        }
        return auditLogDatas;
    }

    public async Task<List<UserActiveDaysAuditLog>> GetUserActivitiesReportAsync(UserActivitiesQueryParameters userActivitiesQueryParameters)
    {
        var tmp = await auditDbContext.AuditLogs!.CountAsync();
        return await auditDbContext.AuditLogs.Where(ad => userActivitiesQueryParameters.UserIds.Contains(ad.UserId)
                                                    && DateOnly.FromDateTime(ad.TimeStamp.Date) <= userActivitiesQueryParameters.ToDate
                                                    && DateOnly.FromDateTime(ad.TimeStamp.Date) >= userActivitiesQueryParameters.FromDate)
                                              .GroupBy(ad => new
                                              {
                                                  ad.UserId,
                                              })
                                              .Select(g => new UserActiveDaysAuditLog
                                              {
                                                  UserId =  g.Key.UserId, 
                                                  ActiveDays = g.Select(ad => ad.TimeStamp.Date).Distinct().Count()
                                              }).ToListAsync();
    }
}
