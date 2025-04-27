using MIDASS.Application.Commons.Models;
using MIDASS.Application.Commons.Models.Users;
using MIDASS.Contract.SharedKernel;

namespace MIDASS.Application.UseCases;

public interface IUserServices
{
    Task<Result<string>> CreateBookBorrowingRequestAsync(BookBorrowingRequestCreate bookBorrowingRequest);
    Task<Result<PaginationResult<BookBorrowingRequestResponse>>> GetBookBorrowingRequestByIdAsync(Guid id, QueryParameters queryParameters);
    Task<Result<UserDetailResponse>> GetByIdAsync(Guid id);
}
