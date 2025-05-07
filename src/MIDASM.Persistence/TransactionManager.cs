
using Microsoft.EntityFrameworkCore.Storage;
using MIDASM.Contract.Messages.ExceptionMessages;
using MIDASM.Domain.Abstract;

namespace MIDASM.Persistence;

public class TransactionManager(ApplicationDbContext context) : ITransactionManager
{
    private IDbContextTransaction? _transaction;

    public async Task BeginTransactionAsync()
    {
        _transaction = await context.BeginTransactionAsync();
    }

    public Task CommitTransactionAsync()
    {
        if (_transaction == null)
        {
            throw new ArgumentException(ApplicationExceptionMessages.CanNotCommitNullTransaction);
        }
        return context.CommitTransactionAsync(_transaction);
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
            throw new ArgumentException(ApplicationExceptionMessages.CanNotRollBackNullTransaction);
        }
        return _transaction.RollbackAsync();
    }

}
