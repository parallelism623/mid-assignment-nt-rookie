using Microsoft.AspNetCore.Mvc;
using MIDASM.Application.Commons.Models.BookReviews;
using MIDASM.Contract.SharedKernel;

namespace MIDASM.Application.UseCases;

public interface IBookReviewServices
{
    Task<Result<string>> CreateBookReviewAsync(CreateBookReviewRequest bookReviewCreateRequest);
    Task<Result<PaginationResult<BookReviewDetailResponse>>> GetAsync(BookReviewQueryParameters bookReviewQueryParameters);
}
