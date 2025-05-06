
using Microsoft.EntityFrameworkCore;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;

namespace MIDASM.Persistence.Repositories;

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
