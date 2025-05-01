using MIDASM.Application.Commons.Models.Books;
using MIDASM.Domain.Entities;

namespace MIDASM.Persistence.Specifications;

public class BookByQueryParametersSpecification : Specification<Book, Guid>
{
    public BookByQueryParametersSpecification(BooksQueryParameters queryParameters) 
        : base(b => (!queryParameters.Availability || b.Available > 0) 
                    && (string.IsNullOrEmpty(queryParameters.Search) || (b.Title.Contains(queryParameters.Search)
                                                                         || b.Author.Contains(queryParameters.Search)
                                                                         || b.Category.Name.Contains(queryParameters.Search))) 
                    && (queryParameters.Ids.Count == 0 
                    || queryParameters.Ids.Contains(b.Id))
                    && (queryParameters.CategoryIds == null || (queryParameters.CategoryIds.Contains(b.CategoryId)))
                    && ((b.BookReviews!.Any() ? (b.BookReviews!.Sum(br => br.Rating)/ b.BookReviews!.Count) : 0) >= queryParameters.Rating))
    {
        AddInclude(b => b.Category);
        AddOrderByDescending(b => b.CreatedAt);

    }

}
