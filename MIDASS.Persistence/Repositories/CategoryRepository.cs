using Microsoft.EntityFrameworkCore;
using MIDASS.Domain.Entities;
using MIDASS.Domain.Repositories;

namespace MIDASS.Persistence.Repositories;

public class CategoryRepository : RepositoryBase<Category, Guid>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<Category?> GetByNameAsync(string name)
    {
        return _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
    }
}
