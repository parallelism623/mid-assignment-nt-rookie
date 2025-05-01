
using MIDASM.Application.Commons.Models.BookBorrowingRequests;
using MIDASM.Contract.SharedKernel;

namespace MIDASM.Application.UseCases;

public interface IBookBorrowingRequestServices
{
    Task<Result<PaginationResult<BookBorrowingRequestData>>> GetsAsync(BookBorrowingRequestQueryParameters queryParameters);
    Task<Result<string>> ChangeStatusAsync(BookBorrowingStatusUpdateRequest statusUpdateRequest);
    Task<Result<BookBorrowingRequestDetailResponse>> GetDetailAsync(Guid id);
}
