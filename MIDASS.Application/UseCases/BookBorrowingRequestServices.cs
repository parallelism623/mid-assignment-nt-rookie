
using MIDASS.Application.Commons.Models.BookBorrowingRequests;
using MIDASS.Contract.SharedKernel;

namespace MIDASS.Application.UseCases;

public interface IBookBorrowingRequestServices
{
    Task<Result<PaginationResult<BookBorrowingRequestData>>> GetsAsync(BookBorrowingRequestQueryParameters queryParameters);
    Task<Result<string>> ChangeStatusAsync(BookBorrowingStatusUpdateRequest statusUpdateRequest);
    Task<Result<BookBorrowingRequestDetailResponse>> GetDetailAsync(Guid id);
}
