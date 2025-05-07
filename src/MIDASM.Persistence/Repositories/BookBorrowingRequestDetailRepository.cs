
using Microsoft.EntityFrameworkCore;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;

namespace MIDASM.Persistence.Repositories;

public class BookBorrowingRequestDetailRepository(ApplicationDbContext context)
    : RepositoryBase<BookBorrowingRequestDetail, Guid>(context),
    IBookBorrowingRequestDetailRepository
{
}
