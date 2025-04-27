using MIDASS.Domain.Entities;

namespace MIDASS.Domain.Repositories;

public interface ICategoryRepository : IRepositoryBase<Category, Guid>
{
    Task<Category?> GetByNameAsync(string name);
}
