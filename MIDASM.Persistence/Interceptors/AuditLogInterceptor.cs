using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MIDASM.Application.Services.Authentication;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Enums;

namespace MIDASM.Persistence.Interceptors;

public class AuditLogInterceptor : SaveChangesInterceptor
{
    private readonly IExecutionContext _executionContext;
    private readonly AuditLogDbContext _auditLogDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly List<AuditLog>? _auditLog = new();
    private readonly List<AuditLogData>? _auditLogData = new();
    public AuditLogInterceptor(IExecutionContext executionContext, 
        AuditLogDbContext auditLogDbContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _executionContext = executionContext;  
        _auditLogDbContext = auditLogDbContext;
    }
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
       DbContextEventData eventData,
       InterceptionResult<int> result,
       CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;

  

        context!.ChangeTracker.Entries()
           .Where(e => e.State == EntityState.Added ||
                       e.State == EntityState.Modified ||
                       e.State == EntityState.Deleted)
            .ToList().ForEach(e => CreateAuditEntry(e));

        if(_auditLog != null && _auditLog.Any())
            _auditLogDbContext.AddRange(_auditLog);
        if(_auditLogData != null && _auditLogData.Any())
            _auditLogDbContext.AddRange(_auditLogData);
        await _auditLogDbContext.SaveChangesAsync();
        return result;
    }
    public override async Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        if (_auditLog != null && _auditLog.Any())
        {
            _auditLogDbContext.Attach(_auditLog);
            foreach (var auditLog in _auditLog)
            {
                auditLog.Status = (int)AuditLogStatus.Failure;
                auditLog.ErrorDescription = eventData.Exception.InnerException?.Message;
            }
        }    

        await _auditLogDbContext.SaveChangesAsync(cancellationToken);
    }
    private void CreateAuditEntry(EntityEntry entry)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var routeData = httpContext?.GetRouteData();
        var model = entry.Metadata.GetTableName();
        var auditEntry = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = _executionContext.GetUserId(),
            BrowserInfo = httpContext?.Request.Headers["User-Agent"].ToString(),
            HttpMethod = httpContext?.Request.Method,
            Url = httpContext?.Request.Path.ToString(),
            ServiceName = routeData?.Values["controller"]?.ToString(),
            MethodName = routeData?.Values["action"]?.ToString(),
            EntityId = entry.OriginalValues[entry.Metadata.FindPrimaryKey()!.Properties.FirstOrDefault()!.Name]!.ToString() ?? null,
            EntityName = model,
            Status = (int)AuditLogStatus.Success,
        };
        _auditLog?.Add(auditEntry);
        foreach (var property in entry.Properties)
        {
            var columnName = property.Metadata.Name;
            var oldValue = property.OriginalValue?.ToString();
            var newValue = property.CurrentValue?.ToString();

            if (!Equals(oldValue, newValue) && (!columnName.Equals("CreatedAt") 
                || !columnName.Equals("CreatedBy") 
                || !columnName.Equals("ModifiedAt") 
                || !columnName.Equals("ModifiedBy")))
            {
                var auditLogData = new AuditLogData();
                auditLogData.Id = Guid.NewGuid();

                auditLogData.AuditLogId = auditEntry.Id;
                auditLogData.PropertyName = columnName;
                auditLogData.PropertyTypeFullName = property.Metadata.ClrType.FullName;
                auditLogData.OriginalValue = oldValue;
                auditLogData.NewValue = newValue;

                _auditLogData?.Add(auditLogData);
            }
        }
    }
}
