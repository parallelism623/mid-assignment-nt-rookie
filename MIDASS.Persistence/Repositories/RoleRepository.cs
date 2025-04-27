
using Microsoft.EntityFrameworkCore;
using MIDASS.Domain.Entities;
using MIDASS.Domain.Repositories;

namespace MIDASS.Persistence.Repositories;

public class RoleRepository : RepositoryBase<Role, Guid>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<Role?> GetByNameAsync(string name)
    {
        return _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }
}
