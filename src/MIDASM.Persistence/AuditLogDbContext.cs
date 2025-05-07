
using Microsoft.EntityFrameworkCore;
using MIDASM.Domain.Entities;

namespace MIDASM.Persistence;

public class AuditLogDbContext
    (DbContextOptions<AuditLogDbContext> options) 
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>().HasIndex(al => al.UserId);
        modelBuilder.Entity<AuditLogData>().HasIndex(al => al.AuditLogId);
        base.OnModelCreating(modelBuilder);
    }
    public DbSet<AuditLog>? AuditLogs { get; set; }
    public DbSet<AuditLogData>? AuditLogDatas { get; set; }
}
