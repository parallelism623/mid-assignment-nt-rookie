
using Microsoft.EntityFrameworkCore;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Enums;
using MIDASM.Domain.Repositories;

namespace MIDASM.Persistence.Repositories;

public class BookBorrowingRequestRepository(ApplicationDbContext context) 
    : RepositoryBase<BookBorrowingRequest, Guid>(context), IBookBorrowingRequestRepository
{
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
