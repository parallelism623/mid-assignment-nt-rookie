
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MIDASS.Domain.Entities;
using System.Data;
using System.Reflection;

namespace MIDASS.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public ApplicationDbContext() { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    public DbSet<Book> Books { get; set; }
    public DbSet<BookBorrowingRequest> BookBorrowingRequests { get; set; }
    public DbSet<BookBorrowingRequestDetail> BookBorrowingRequestDetails { get; set; }
    public DbSet<Category> Categories { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    private IDbContextTransaction? _transaction;

    public IDbContextTransaction? GetCurrentTransaction() => _transaction;

    private bool HasActiveTransaction => _transaction != null;
    public async Task<IDbContextTransaction?> BeginTransactionAsync()
    {
        if (_transaction != null)
        {
            return null;
        }
        _transaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        return _transaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        if (_transaction == null)
        {
            throw new ArgumentException("Current transaction in dbcontext is null");
        }
        if (_transaction != transaction)
        {
            throw new ArgumentException("Current transaction not match");
        }
        try
        {
            await _transaction.CommitAsync();
        }
        catch
        {
            Rollback();
            throw;
        }
        finally
        {
            if (HasActiveTransaction)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }

    public void Rollback()
    {
        try
        {
            _transaction?.Rollback();
        }
        finally
        {
            if (HasActiveTransaction)
            {
                _transaction!.Dispose();
                _transaction = null;
            }
        }
    }
}
