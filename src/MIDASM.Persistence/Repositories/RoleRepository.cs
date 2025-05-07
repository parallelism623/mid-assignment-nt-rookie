
using Microsoft.EntityFrameworkCore;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;

namespace MIDASM.Persistence.Repositories;

public class RoleRepository(ApplicationDbContext context) : RepositoryBase<Role, Guid>(context), IRoleRepository
{
    public Task<Role?> GetByNameAsync(string name)
    {
        return _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }
}
