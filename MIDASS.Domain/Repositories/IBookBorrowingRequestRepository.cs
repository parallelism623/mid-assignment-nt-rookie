using MIDASS.Domain.Entities;

namespace MIDASS.Domain.Repositories;

public interface IBookBorrowingRequestRepository : IRepositoryBase<BookBorrowingRequest, Guid>
{
    public Task<BookBorrowingRequest?> GetDetailAsync(Guid id);
    public Task<BookBorrowingRequest?> FindByBookBorrowedOfUserAsync(Guid userId , Guid bookId);
}
