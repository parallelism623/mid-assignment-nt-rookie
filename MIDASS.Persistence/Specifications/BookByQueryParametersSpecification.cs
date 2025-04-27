using MIDASS.Application.Commons.Models.Books;
using MIDASS.Domain.Entities;

namespace MIDASS.Persistence.Specifications;

public class BookByQueryParametersSpecification : Specification<Book, Guid>
{
    public BookByQueryParametersSpecification(BooksQueryParameters queryParameters) 
        : base(b => (!queryParameters.Availability || b.Available > 0) 
                    && (string.IsNullOrEmpty(queryParameters.Search) || (b.Title.Contains(queryParameters.Search)
                                                                         || b.Author.Contains(queryParameters.Search)
                                                                         || b.Category.Name.Contains(queryParameters.Search))) 
                    && (queryParameters.Ids.Count == 0 
                    || queryParameters.Ids.Contains(b.Id)))
    {
        AddInclude(b => b.Category);
    }
}
