
using Microsoft.EntityFrameworkCore;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;

namespace MIDASM.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context) 
    : RepositoryBase<User, Guid>(context), IUserRepository
{

    public Task<User?> GetByUsernameAsync(string userName, params string[] includes)
    {
        var query = _context.Users.Where(u => u.Username == userName);
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query.FirstOrDefaultAsync();
    }

    public Task<User?> GetByEmailAsync(string email, params string[] includes)
    {
        var query = _context.Users.Where(u => u.Email == email);
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query.FirstOrDefaultAsync();
    }
}
