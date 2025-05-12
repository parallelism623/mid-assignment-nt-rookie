using MIDASM.Application.Commons.Models.BookBorrowingRequests;
using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.UseCases.Interfaces;

public interface IBookBorrowingRequestServices
{
    Task<Result<PaginationResult<BookBorrowingRequestData>>> GetsAsync(BookBorrowingRequestQueryParameters queryParameters);
    Task<Result<string>> ChangeStatusAsync(BookBorrowingStatusUpdateRequest statusUpdateRequest);
    Task<Result<BookBorrowingRequestDetailResponse>> GetDetailAsync(Guid id);
}
