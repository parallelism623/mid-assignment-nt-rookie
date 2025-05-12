
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MIDASM.Contract.Messages.ExceptionMessages;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;
using System.Data;
using System.Reflection;

namespace MIDASM.Persistence;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public ApplicationDbContext() { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<EmailRecord> EmailRecords { get; set; }  
    public DbSet<Book> Books { get; set; }
    public DbSet<BookBorrowingRequest> BookBorrowingRequests { get; set; }
    public DbSet<BookBorrowingRequestDetail> BookBorrowingRequestDetails { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BookReview> BookReviews { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    private IDbContextTransaction? _transaction;

    private bool HasActiveTransaction => _transaction != null;



    public async Task BeginTransactionAsync()
    {
        _transaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
    }

    public async Task RollbackAsync()
    {
        if (_transaction == null)
        {
            throw new ArgumentException("Cannot roll back empty transaction!");
        }
        try
        {
            await _transaction!.RollbackAsync();
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

    public async Task CommitTransactionAsync()
    {
        if (_transaction == null)
        {
            throw new ArgumentException(ApplicationExceptionMessages.CurrentTransactionNull);
        }
        try
        {
            await _transaction.CommitAsync();
        }
        catch
        {
            await RollbackAsync();
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

}
