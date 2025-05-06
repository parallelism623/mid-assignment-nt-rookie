
using MIDASM.Domain.Entities;

namespace MIDASM.Domain.Repositories;

public interface IBookRepository : IRepositoryBase<Book, Guid>
{
    Task<List<Book>> GetByIdsAsync(IEnumerable<Guid> ids);
}
