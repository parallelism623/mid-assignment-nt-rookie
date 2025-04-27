using MIDASS.Domain.Abstract;
using System.Linq.Expressions;

namespace MIDASS.Domain.Repositories;

public interface IRepositoryBase<TEntity, TKey>
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
}
