
using MIDASM.Domain.Entities;

namespace MIDASM.Domain.Repositories;

public interface IAuditLoggerRepository
{
    void AddRange(IEnumerable<AuditLogData> auditLogDatas);
    void Add(AuditLog auditLog);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    IQueryable<AuditLog> GetAuditLogQueryable();
    IQueryable<AuditLogData> GetAuditLogDataQueryable();

    Task<int> CountAsync<T>(IQueryable<T> queryable);
    Task<List<T>> ToListAsync<T>(IQueryable<T> queryable);
}
