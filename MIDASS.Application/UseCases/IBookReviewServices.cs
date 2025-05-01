using Microsoft.AspNetCore.Mvc;
using MIDASS.Application.Commons.Models.BookReviews;
using MIDASS.Contract.SharedKernel;

namespace MIDASS.Application.UseCases;

public interface IBookReviewServices
{
    Task<Result<string>> CreateBookReviewAsync(CreateBookReviewRequest bookReviewCreateRequest);
    Task<Result<PaginationResult<BookReviewDetailResponse>>> GetAsync(BookReviewQueryParameters bookReviewQueryParameters);
}
