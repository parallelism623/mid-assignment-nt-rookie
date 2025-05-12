
using Microsoft.EntityFrameworkCore;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;
using System.Security.AccessControl;

namespace MIDASM.Persistence.Repositories;

public class AuditLoggerRepository(AuditLogDbContext auditLogDbContext)
    : IAuditLoggerRepository
{
    
    public void AddRange(IEnumerable<AuditLogData> auditLogDatas)
    {
        auditLogDbContext.AddRange(auditLogDatas);
    }

    public void Add(AuditLog auditLog)
    {
        auditLogDbContext.Add(auditLog);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await auditLogDbContext.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<AuditLog> GetAuditLogQueryable()
    {
        return auditLogDbContext.AuditLogs!.AsNoTracking();
    }

    public IQueryable<AuditLogData> GetAuditLogDataQueryable()
    {
        return auditLogDbContext.AuditLogDatas!.AsNoTracking();
    }

    public Task<int> CountAsync<T>(IQueryable<T> queryable)
    {
        return queryable.CountAsync();
    }

    public  Task<List<T>> ToListAsync<T>(IQueryable<T> queryable)
    {
        return queryable.ToListAsync();
    }
}
