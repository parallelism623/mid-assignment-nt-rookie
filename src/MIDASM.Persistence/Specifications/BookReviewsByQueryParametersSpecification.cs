
using MIDASM.Application.Commons.Models.BookReviews;
using MIDASM.Domain.Entities;

namespace MIDASM.Persistence.Specifications;

public class BookReviewsByQueryParametersSpecification : Specification<BookReview, Guid>
{
    public BookReviewsByQueryParametersSpecification(BookReviewQueryParameters queryParameters) : 
        base(br =>(queryParameters.BookId == null || br.BookId == queryParameters.BookId) 
                && (queryParameters.ReviewId == null || br.ReviewerId == queryParameters.ReviewId)
                && (queryParameters.Rating.Length == 0 || queryParameters.Rating.Contains(br.Rating)))
    {
        AddInclude(br => br.Reviewer);
        AddInclude(br => br.Book);
        AddOrderByDescending(b => b.DateReview);
    }
}
