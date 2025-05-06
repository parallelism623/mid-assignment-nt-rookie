using MIDASM.Application.Commons.Models;
using MIDASM.Application.Commons.Models.BookBorrowingRequestDetails;
using MIDASM.Contract.SharedKernel;

namespace MIDASM.Application.UseCases;

public interface IBookBorrowingRequestDetailServices
{
    Task<Result<PaginationResult<BookBorrowedRequestDetailResponse>>> GetsAsync(QueryParameters queryParameters);
    Task<Result<string>> AdjustExtendDueDateAsync(Guid id, int status);
}
