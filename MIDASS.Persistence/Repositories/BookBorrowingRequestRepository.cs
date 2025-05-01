
using Microsoft.EntityFrameworkCore;
using MIDASS.Domain.Entities;
using MIDASS.Domain.Enums;
using MIDASS.Domain.Repositories;

namespace MIDASS.Persistence.Repositories;

public class BookBorrowingRequestRepository : RepositoryBase<BookBorrowingRequest, Guid>, IBookBorrowingRequestRepository
{
    public BookBorrowingRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<BookBorrowingRequest?> FindByBookBorrowedOfUserAsync(Guid userId, Guid bookId)
    {
        return _dbSet.FirstOrDefaultAsync(bq => bq.RequesterId == userId
        && bq.Status == (int)BookBorrowingStatus.Approved
        && bq.BookBorrowingRequestDetails.Any(bd => bd.BookId == bookId));
    }

    public Task<BookBorrowingRequest?> GetDetailAsync(Guid id)
    {
        return _context.BookBorrowingRequests
            .AsNoTracking()
            .AsSplitQuery()
            .Where(b => b.Id == id)
            .Include(b => b.BookBorrowingRequestDetails)
            .ThenInclude(bd => bd.Book)
            .ThenInclude(b => b.Category).FirstOrDefaultAsync();
    }

}
