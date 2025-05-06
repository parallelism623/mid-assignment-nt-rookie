using MIDASM.Domain.Entities;

namespace MIDASM.Domain.Repositories;

public interface IRoleRepository : IRepositoryBase<Role, Guid>
{
    public Task<Role?> GetByNameAsync(string name);
}
