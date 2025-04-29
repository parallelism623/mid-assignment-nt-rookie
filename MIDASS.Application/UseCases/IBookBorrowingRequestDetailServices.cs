using MIDASS.Application.Commons.Models;
using MIDASS.Application.Commons.Models.BookBorrowingRequestDetails;
using MIDASS.Contract.SharedKernel;

namespace MIDASS.Application.UseCases;

public interface IBookBorrowingRequestDetailServices
{
    Task<Result<PaginationResult<BookBorrowedRequestDetailResponse>>> GetsAsync(QueryParameters queryParameters);
    Task<Result<string>> AdjustExtenDueDateAsync(Guid id, int status);
}
