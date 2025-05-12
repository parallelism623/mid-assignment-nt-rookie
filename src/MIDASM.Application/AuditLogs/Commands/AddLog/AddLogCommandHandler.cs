
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MIDASM.Application.Dispatcher.Commands;
using MIDASM.Application.Services.Authentication;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;
using System.Net;

namespace MIDASM.Application.AuditLogs.Commands.AddLog;

public class AddLogCommandHandler(IAuditLoggerRepository auditLoggerRepository,
    IExecutionContext executionContext,
    IHttpContextAccessor httpContextAccessor) : ICommandHandler<AddLogCommand>
{
    public async Task HandlerAsync(AddLogCommand request, CancellationToken cancellationToken = default)
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
            EntityId = request.EntityId,
            EntityName = request.EntityName,
            TimeStamp = DateTime.UtcNow,
            Description = request.Description,
        };
        auditLoggerRepository.Add(auditLog);
        var auditLogDatas = GetAuditLogDataFromChangedProperties(auditLog.Id, request.ChangedProperties);
        if (auditLogDatas.Count > 0)
        {
            auditLoggerRepository.AddRange(auditLogDatas);
        }
        await auditLoggerRepository.SaveChangesAsync();
    
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
}
