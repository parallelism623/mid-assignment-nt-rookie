
using Microsoft.EntityFrameworkCore;
using MIDASS.Domain.Entities;
using MIDASS.Domain.Repositories;

namespace MIDASS.Persistence.Repositories;

public class BookRepository : RepositoryBase<Book,Guid>, IBookRepository
{
    public BookRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<List<Book>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return _context.Books.Where(b => ids.Contains(b.Id))
            .ToListAsync();
    }
}
