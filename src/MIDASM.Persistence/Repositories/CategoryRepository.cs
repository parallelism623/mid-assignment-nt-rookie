using Microsoft.EntityFrameworkCore;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;

namespace MIDASM.Persistence.Repositories;

public class CategoryRepository(ApplicationDbContext context): RepositoryBase<Category, Guid>(context), ICategoryRepository
{

    public Task<Category?> GetByNameAsync(string name)
    {
        return _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
    }
}
