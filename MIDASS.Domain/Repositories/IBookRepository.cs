
using MIDASS.Domain.Entities;

namespace MIDASS.Domain.Repositories;

public interface IBookRepository : IRepositoryBase<Book, Guid>
{
    Task<List<Book>> GetByIdsAsync(IEnumerable<Guid> ids);
}
