using MIDASM.Domain.Entities;

namespace MIDASM.Domain.Repositories;

public interface ICategoryRepository : IRepositoryBase<Category, Guid>
{
    Task<Category?> GetByNameAsync(string name);
}
