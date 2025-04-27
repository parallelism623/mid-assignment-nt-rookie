
using Microsoft.EntityFrameworkCore;
using MIDASS.Domain.Entities;
using MIDASS.Domain.Repositories;

namespace MIDASS.Persistence.Repositories;

public class UserRepository : RepositoryBase<User, Guid>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

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
