
using Microsoft.EntityFrameworkCore;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;

namespace MIDASM.Persistence.Repositories;

public class BookBorrowingRequestDetailRepository
    : RepositoryBase<BookBorrowingRequestDetail, Guid>,
    IBookBorrowingRequestDetailRepository
{
    public BookBorrowingRequestDetailRepository(ApplicationDbContext context) : base(context)
    {
    }

}
