
using Microsoft.EntityFrameworkCore;
using MIDASS.Application.Commons.Mapping;
using MIDASS.Application.Commons.Models.BookReviews;
using MIDASS.Application.UseCases;
using MIDASS.Contract.Errors;
using MIDASS.Contract.Messages.Commands;
using MIDASS.Contract.SharedKernel;
using MIDASS.Domain.Entities;
using MIDASS.Domain.Repositories;
using MIDASS.Persistence.Specifications;

namespace MIDASS.Persistence.Services;

public class BookReviewServices(IBookReviewRepository bookReviewRepository, 
                                IBookBorrowingRequestRepository bookBorrowingRequestRepository,
                                IUserRepository userRepository,
                                IBookRepository bookRepository) 
                                : IBookReviewServices
{
    public async Task<Result<string>> CreateBookReviewAsync(CreateBookReviewRequest bookReviewCreateRequest)
    {
        var reviewerId = bookReviewCreateRequest.ReviewerId;
        var bookId = bookReviewCreateRequest.BookId;
        var book = await bookRepository.GetByIdAsync(bookId);
        if(book == null)
        {
            return Result<string>.Failure(400, BookReviewErrors.BookNotExists);
        }
        var reviewer = await userRepository.GetByIdAsync(reviewerId);
        if (reviewer == null)
        {
            return Result<string>.Failure(400, BookReviewErrors.ReviewerNotExists);
        }
        var userBookBorrowingRequest = await bookBorrowingRequestRepository.FindByBookBorrowedOfUserAsync(reviewerId, bookId);
        if (userBookBorrowingRequest == null)
        {
            return Result<string>.Failure(400, BookReviewErrors.UserHasNotBorrowedBook);
        }
      
        var bookReview = BookReview.Create(bookReviewCreateRequest.ReviewerId,
                                           bookReviewCreateRequest.BookId,
                                           bookReviewCreateRequest.Title,
                                           bookReviewCreateRequest.Content,
                                           bookReviewCreateRequest.DateReview,
                                           bookReviewCreateRequest.Rating);
        bookReviewRepository.Add(bookReview);
        await bookReviewRepository.SaveChangesAsync();

        return BookReviewCommandMessages.CreateSuccess;
    }

    public async Task<Result<PaginationResult<BookReviewDetailResponse>>> GetAsync(BookReviewQueryParameters bookReviewQueryParameters)
    {
        var query = bookReviewRepository.GetQueryable();
        var querySpecification = new BookReviewsByQueryParametersSpecification(bookReviewQueryParameters);

        query = querySpecification.GetQuery(query);
            
        var totalCount = await query.CountAsync();

        var bookReviews = await query.Skip(bookReviewQueryParameters.Skip)
                                     .Take(bookReviewQueryParameters.Take)
                                     .ToListAsync();

        var bookReviewResponses = bookReviews.Select(br => br.ToBookReviewDetailResponse()).ToList();


        return PaginationResult<BookReviewDetailResponse>.Create(10, 1, totalCount, bookReviewResponses);

    }
}
