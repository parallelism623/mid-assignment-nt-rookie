using MIDASM.Domain.Abstract;
using System.Linq.Expressions;

namespace MIDASM.Domain.Repositories;

public interface IRepositoryBase<TEntity, TKey> : IConcurrencyHandler<TEntity>
    where TEntity : IEntity<TKey>
    where TKey : notnull
{
    Task<TEntity?> GetByIdAsync(TKey id, params string[] includes);
    Task<TEntity?> GetByIdAsync(TKey id, Expression<Func<TEntity, object>> include);
    void Add(TEntity entity);
    void Delete(TEntity entity);
    void Update(TEntity entity);
    Task<List<TEntity>> GetByIdsAsync(List<TKey> ids);
    void UpdateRange(IEnumerable<TEntity> entities);
    Task SaveChangesAsync();
    IQueryable<TEntity> GetQueryable();

    Task<List<TEntity>> GetAll();
    Task<int> CountAsync(IQueryable<TEntity> queryable);
    Task<int> CountAsync<T>(IQueryable<T> queryable);
    Task<List<TEntity>> ToListAsync(IQueryable<TEntity> queryable);
    Task<List<T>> ToListAsync<T>(IQueryable<T> queryable);
}
