
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MIDASM.Application.Services.Authentication;
using MIDASM.Domain.Abstract;
using System.Threading;

namespace MIDASM.Persistence.Interceptors;

public class AuditableEntitiesInterceptor: SaveChangesInterceptor
{
    private readonly IExecutionContext _executionContext;

    public AuditableEntitiesInterceptor(IExecutionContext executionContext)
    {
        _executionContext = executionContext;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        DbContext? dbContext = eventData.Context;

        if (dbContext is null)
        {
            return base.SavingChangesAsync(
                eventData,
                result,
                cancellationToken);
        }

        IEnumerable<EntityEntry<AuditableEntity>> entries =
            dbContext
                .ChangeTracker
                .Entries<AuditableEntity>();

        foreach (EntityEntry<AuditableEntity> entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(a => a.CreatedBy).CurrentValue = _executionContext.GetUserId();
                entityEntry.Property(a => a.CreatedAt).CurrentValue = DateTime.UtcNow;
            } 

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(a => a.ModifiedBy).CurrentValue = _executionContext.GetUserId();
                entityEntry.Property(a => a.ModifiedAt).CurrentValue = DateTime.UtcNow;
            }
        }

        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }
}
