
using Microsoft.EntityFrameworkCore;
using MIDASS.Domain.Entities;
using MIDASS.Domain.Repositories;

namespace MIDASS.Persistence.Repositories;

public class BookBorrowingRequestDetailRepository
    : RepositoryBase<BookBorrowingRequestDetail, Guid>,
    IBookBorrowingRequestDetailRepository
{
    public BookBorrowingRequestDetailRepository(ApplicationDbContext context) : base(context)
    {
    }

}
