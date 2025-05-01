
using Microsoft.EntityFrameworkCore.Storage;
using MIDASM.Domain.Abstract;

namespace MIDASM.Persistence;

public class TransactionManager : ITransactionManager
{
    private readonly ApplicationDbContext _context;
    public TransactionManager(ApplicationDbContext context)
    {
        _context = context;
    }
    private IDbContextTransaction? _transaction;

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.BeginTransactionAsync();
    }

    public Task CommitTransactionAsync()
    {
        if (_transaction == null)
        {
            throw new ArgumentException("Transaction (in Transaction Manager) null, can not commit");
        }
        return _context.CommitTransactionAsync(_transaction);
    }

    public void DisposeTransaction()
    {
        _transaction?.Dispose();
        _transaction = null;
    }

    public Task RollbackAsync()
    {
        if (_transaction == null)
        {
            throw new ArgumentException("Transaction (in Transaction Manager) null, can not commit");
        }
        return _transaction.RollbackAsync();
    }

}
