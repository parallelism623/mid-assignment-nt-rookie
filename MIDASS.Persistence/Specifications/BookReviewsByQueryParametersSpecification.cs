
using MIDASS.Application.Commons.Models.BookReviews;
using MIDASS.Domain.Entities;

namespace MIDASS.Persistence.Specifications;

public class BookReviewsByQueryParametersSpecification : Specification<BookReview, Guid>
{
    public BookReviewsByQueryParametersSpecification(BookReviewQueryParameters queryParameters) : 
        base(br => (br.Title.Contains(queryParameters.Search)) 
                && (string.IsNullOrEmpty(br.Content) || br.Content.Contains(queryParameters.Search))
                && (queryParameters.BookId == null || br.BookId == queryParameters.BookId) 
                && (queryParameters.ReviewId == null || br.ReviewerId == queryParameters.ReviewId)
                && (queryParameters.Rating.Length == 0 || queryParameters.Rating.Contains(br.Rating)))
    {
        AddInclude(br => br.Reviewer);
        AddInclude(br => br.Book);
        AddOrderByDescending(b => b.DateReview);
    }
}
