
using Microsoft.EntityFrameworkCore;
using MIDASM.Domain.Abstract;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;
using System;
using System.Linq.Expressions;

namespace MIDASM.Persistence.Repositories;

public abstract class RepositoryBase<TEntity, TKey> : IRepositoryBase<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    protected RepositoryBase(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }
    public Task<TEntity?> GetByIdAsync(TKey id, params string[] includes)
    {
        var query = _context.Set<TEntity>().Where(t => t.Id.Equals(id));
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query.FirstOrDefaultAsync();
    }

    public Task<TEntity?> GetByIdAsync(TKey id, Expression<Func<TEntity, object>> include)
    {
        var query = _context.Set<TEntity>().Where(t => t.Id.Equals(id));

        query = query.Include(include);
        

        return query.FirstOrDefaultAsync();
    }

    public void Add(TEntity entity)
    {
        _context.Add(entity);
    }

    public void Delete(TEntity entity)
    {
        _context.Remove(entity);
    }

    public void Update(TEntity entity)
    {
        _context.Update(entity);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            Update(entity);
        }
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public IQueryable<TEntity> GetQueryable()
    {
        return _dbSet.AsNoTracking();
    }

    public Task<List<TEntity>> GetByIdsAsync(List<TKey> ids)
    {
        return _dbSet.AsNoTracking().Where(b => ids.Contains(b.Id)).ToListAsync();
    }

    public Task<List<TEntity>> GetAll()
    {
        return _dbSet.AsNoTracking().ToListAsync();
    }

    public Task<int> CountAsync(IQueryable<TEntity> queryable)
    {
        return queryable.CountAsync();
    }

    public Task<int> CountAsync<T>(IQueryable<T> queryable)
    {
        return queryable.CountAsync();
    }

    public Task<List<TEntity>> ToListAsync(IQueryable<TEntity> queryable)
    {
        return queryable.ToListAsync();
    }

    public Task<List<T>> ToListAsync<T>(IQueryable<T> queryable)
    {
        return queryable.ToListAsync();
    }

    public bool IsDbUpdateConcurrencyException(Exception ex)
    {
        return ex is DbUpdateConcurrencyException;
    }

    public async Task ApplyUpdatedValuesFromDataSource(Exception ex)
    {
        var concurrencyException = (ex as DbUpdateConcurrencyException);
        var bookEntry = concurrencyException!.Entries?.Where(entity => entity.Entity is TEntity)?.ToList();
        if (bookEntry != null)
        {
            foreach (var entry in bookEntry)
            {

                var databaseValues = await entry.GetDatabaseValuesAsync();
                if (databaseValues != null)
                {
                    entry.OriginalValues.SetValues(databaseValues);
                    entry.CurrentValues.SetValues(databaseValues);
                }

            }
        }
    }
}
