
using Microsoft.EntityFrameworkCore;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;

namespace MIDASM.Persistence.Repositories;

public class BookRepository(ApplicationDbContext context) 
    : RepositoryBase<Book,Guid>(context), IBookRepository
{
    public Task<List<Book>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return _context.Books.Where(b => ids.Contains(b.Id))
            .ToListAsync();
    }
}
