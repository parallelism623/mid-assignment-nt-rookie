
using MIDASS.Domain.Entities;
using MIDASS.Domain.Repositories;

namespace MIDASS.Persistence.Repositories;

public class BookBorrowingRequestRepository : RepositoryBase<BookBorrowingRequest, Guid>, IBookBorrowingRequestRepository
{
    public BookBorrowingRequestRepository(ApplicationDbContext context) : base(context)
    {
    }
}
