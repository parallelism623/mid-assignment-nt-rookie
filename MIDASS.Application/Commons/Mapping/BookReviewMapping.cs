
using MIDASS.Application.Commons.Models.BookReviews;
using MIDASS.Application.Commons.Models.Books;
using MIDASS.Domain.Entities;

namespace MIDASS.Application.Commons.Mapping;

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
            Book = new()
            {
                Id = bookResponse.Book.Id,
                Title = bookResponse.Book.Title,
                Author = bookResponse.Book.Author,
            },
            Reviewer = new()
            {
                Id = bookResponse.Reviewer.Id,
                FirstName = bookResponse.Reviewer.FirstName,
                LastName = bookResponse.Reviewer.LastName,
                Username = bookResponse.Reviewer.Username,
            },
            Rating = bookResponse.Rating
        };
    }
}
