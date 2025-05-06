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


namespace MIDASM.Persistence.Services;

public class AuditLogger : IAuditLogger
{
    private readonly AuditLogDbContext _auditDbContext;
    private readonly IExecutionContext _executionContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AuditLogger(AuditLogDbContext auditDbContext, IExecutionContext executionContext,
         IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _executionContext = executionContext;
        _auditDbContext = auditDbContext;
    }
    public async Task<Result<PaginationResult<AuditLogResponse>>> GetActivitiesAsync(AuditLogQueryParameters queryParameters)
    {
        var query = _auditDbContext.AuditLogs.AsQueryable();
        var querySpecification = new AuditLogByQueryParametersSpecification(queryParameters);
        var queryAuditLogData = _auditDbContext.AuditLogDatas.AsQueryable();
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
        var query = _auditDbContext.AuditLogs.AsQueryable();
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
        var httpContext = _httpContextAccessor.HttpContext;
        var routeData = httpContext?.GetRouteData();
        var auditLog = new AuditLog()
        {
            Id = Guid.NewGuid(),
            UserId = _executionContext.GetUserId(),
            BrowserInfo = httpContext?.Request.Headers["User-Agent"].ToString(),
            HttpMethod = httpContext?.Request.Method,
            Username = _executionContext.GetUserName(),
            Url = httpContext?.Request.Path.ToString(),
            ServiceName = routeData?.Values["controller"]?.ToString(),
            MethodName = routeData?.Values["action"]?.ToString()?.Replace("Asycn", ""),
            EntityId = entityId,
            EntityName = entityName,
            TimeStamp = DateTime.UtcNow,
            Description = desciption,
        };
        _auditDbContext.Add(auditLog);
        var auditLogDatas = GetAuditLogDataFromChangedProperties(auditLog.Id, changedProperties);
        if (auditLogDatas.Count > 0)
        {
            _auditDbContext.AddRange(auditLogDatas);
        }
        await _auditDbContext.SaveChangesAsync();
    }


    private static List<AuditLogData> GetAuditLogDataFromChangedProperties(Guid auditLogId, Dictionary<string, (string?, string?)>? changedProperties)
    {
        var auditLogDatas = new List<AuditLogData>();
        if (changedProperties != null)
        {
            foreach (var it in changedProperties)
            {
                var auditLogData = new AuditLogData();
                auditLogData.Id = Guid.NewGuid();

                auditLogData.AuditLogId = auditLogId;
                auditLogData.PropertyName = it.Key;
                auditLogData.OriginalValue = it.Value.Item1;
                auditLogData.NewValue = it.Value.Item2;
                auditLogDatas.Add(auditLogData);
            }
        }
        return auditLogDatas;
    }

    public async Task<List<UserActiveDaysAuditLog>> GetUserActivitiesReportAsync(UserActivitiesQueryParameters userActivitiesQueryParameters)
    {
        var tmp = await _auditDbContext.AuditLogs.CountAsync();
        return await _auditDbContext.AuditLogs.Where(ad => userActivitiesQueryParameters.UserIds.Contains(ad.UserId)
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
