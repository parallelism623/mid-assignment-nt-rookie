
    using Microsoft.EntityFrameworkCore;
    using MIDASS.Domain.Abstract;
    using System.Linq;

    namespace MIDASS.Persistence.Specifications;

    public static class SpecificationsExecutor
    {
        public static IQueryable<TEntity> GetQuery<TEntity, TKey>(
            this Specification<TEntity, TKey> specification,
            IQueryable<TEntity> inputQueryable)
            where TEntity : class, IEntity<TKey>
            where TKey : notnull
        {
            IQueryable<TEntity> queryable = inputQueryable;

            if (specification.Criteria != null)
            {
                queryable = queryable.Where(specification.Criteria);
            }

            if (specification.IncludeExpressions.Count > 0)
            {
                foreach (var item in specification.IncludeExpressions)
                {
                    queryable = queryable.Include(item);
                }
            }

            if (specification.OrderByExpression is not null)
            {
                queryable = queryable.OrderBy(specification.OrderByExpression);
            }
            else if (specification.OrderByDescendingExpression is not null)
            {
                queryable = queryable.OrderByDescending(
                    specification.OrderByDescendingExpression);
            }

            if (specification.IsSplitQuery)
            {
                queryable = queryable.AsSplitQuery();
            }

            return queryable;
        }
    }
