using MIDASM.Application.Commons.Models.BookReviews;
using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.UseCases.Interfaces;

public interface IBookReviewServices
{
    Task<Result<string>> CreateBookReviewAsync(CreateBookReviewRequest bookReviewCreateRequest);
    Task<Result<PaginationResult<BookReviewDetailResponse>>> GetAsync(BookReviewQueryParameters bookReviewQueryParameters);
}
