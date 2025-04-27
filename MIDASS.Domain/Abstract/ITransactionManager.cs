namespace MIDASS.Domain.Abstract;

public interface ITransactionManager
{
    public Task BeginTransactionAsync();
    public Task RollbackAsync();
    public Task CommitTransactionAsync();

    public void DisposeTransaction();
}