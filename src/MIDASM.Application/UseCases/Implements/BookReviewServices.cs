using MIDASM.Application.Commons.Errors;
using MIDASM.Application.Commons.Mapping;
using MIDASM.Application.Commons.Models.BookReviews;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.Services.Authentication;
using MIDASM.Application.UseCases.Interfaces;
using MIDASM.Contract.Helpers;
using MIDASM.Contract.Messages.AuditLogMessage;
using MIDASM.Contract.Messages.Commands;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;
using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.UseCases.Implements;

public class BookReviewServices(IBookReviewRepository bookReviewRepository,
                                IBookBorrowingRequestRepository bookBorrowingRequestRepository,
                                IUserRepository userRepository,
                                IAuditLogger auditLogger,
                                IBookRepository bookRepository,
                                IExecutionContext executionContext)
                                : IBookReviewServices
{
    public async Task<Result<string>> CreateBookReviewAsync(CreateBookReviewRequest bookReviewCreateRequest)
    {
        var reviewerId = bookReviewCreateRequest.ReviewerId;
        var bookId = bookReviewCreateRequest.BookId;
        var book = await bookRepository.GetByIdAsync(bookId);
        if (book == null)
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

        await AddBookReviewIntoStorageAsync(bookReview);

        await HandleAuditLogBookReviewCreate(bookReview);
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
    private async Task HandleAuditLogBookReviewCreate(BookReview bookReview)
    {
        await auditLogger.LogAsync(
            bookReview.Id.ToString(),
            nameof(BookReview),
            StringHelper.ReplacePlaceholders(
                AuditLogMessageTemplate.Create,
                executionContext.GetUserName(),
                "book review",
                $"with rating {bookReview.Rating} stars",
                bookReview.CreatedAt.ToShortTime()
                ),
            GetChangedBookReviewProperties(bookReview));
    }

    private static Dictionary<string, (string?, string?)> GetChangedBookReviewProperties(BookReview newReview, BookReview? oldReview = default)
    {
        var changes = new Dictionary<string, (string?, string?)>();

        if (oldReview == null || newReview.ReviewerId != oldReview.ReviewerId)
            changes.Add(nameof(newReview.ReviewerId), (oldReview?.ReviewerId.ToString(), newReview.ReviewerId.ToString()));

        if (oldReview == null || newReview.BookId != oldReview.BookId)
            changes.Add(nameof(newReview.BookId), (oldReview?.BookId.ToString(), newReview.BookId.ToString()));

        if (oldReview == null || newReview.Rating != oldReview.Rating)
            changes.Add(nameof(newReview.Rating), (oldReview?.Rating.ToString(), newReview.Rating.ToString()));

        if (oldReview == null || newReview.Title != oldReview.Title)
            changes.Add(nameof(newReview.Title), (oldReview?.Title, newReview.Title));

        if (oldReview == null || newReview.Content != oldReview.Content)
            changes.Add(nameof(newReview.Content), (oldReview?.Content, newReview.Content));

        if (oldReview == null || newReview.DateReview != oldReview.DateReview)
            changes.Add(nameof(newReview.DateReview), (oldReview?.DateReview.ToString(), newReview.DateReview.ToString()));

        return changes;
    }


    private async Task AddBookReviewIntoStorageAsync(BookReview bookReview)
    {
        bookReviewRepository.Add(bookReview);
        await bookReviewRepository.SaveChangesAsync();

    }


}

