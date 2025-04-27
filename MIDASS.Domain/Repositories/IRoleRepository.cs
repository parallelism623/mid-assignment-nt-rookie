using MIDASS.Domain.Entities;

namespace MIDASS.Domain.Repositories;

public interface IRoleRepository : IRepositoryBase<Role, Guid>
{
    public Task<Role?> GetByNameAsync(string name);
}
