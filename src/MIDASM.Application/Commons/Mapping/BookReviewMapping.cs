
using MIDASM.Application.Commons.Models.BookReviews;
using MIDASM.Application.Commons.Models.Books;
using MIDASM.Domain.Entities;

namespace MIDASM.Application.Commons.Mapping;

public static class BookReviewMapping
{
    public static BookReviewDetailResponse ToBookReviewDetailResponse(this BookReview bookResponse)
    {
        return new()
        {
            Id = bookResponse.Id,
            DateReview = bookResponse.DateReview,
            Content = bookResponse.Content ?? string.Empty,
            Title = bookResponse.Title,
            Book = bookResponse.Book != null ? new()
            {
                Id = bookResponse.Book.Id,
                Title = bookResponse.Book.Title,
                Author = bookResponse.Book.Author,
            } : default!,
            Reviewer = bookResponse.Reviewer != null ? new()
            {
                Id = bookResponse.Reviewer.Id,
                FirstName = bookResponse.Reviewer.FirstName,
                LastName = bookResponse.Reviewer.LastName,
                Username = bookResponse.Reviewer.Username,
            } : default!,
            Rating = bookResponse.Rating
        };
    }
}
