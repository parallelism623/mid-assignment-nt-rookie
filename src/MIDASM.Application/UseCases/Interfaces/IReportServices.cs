using MIDASM.Application.Commons.Models.Report;
using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.UseCases.Interfaces;

public interface IReportServices
{
    Task<Result<PaginationResult<CategoryReportResponse>>> GetCategoryReportAsync(CategoryReportQueryParameters queryParameters);
    Task<Result<PaginationResult<BookBorrowingReportResponse>>> GetBookBorrowingReportAsync(BookBorrowingReportQueryParameters queryParameters);

    Task<Result<PaginationResult<UserReportResponse>>> GetUserReportAsync(UserEngagementReportQueryParameters queryParameters);
}
