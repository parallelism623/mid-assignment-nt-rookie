
using MIDASM.Application.Commons.Models.Books;
using MIDASM.Application.Dispatcher.Queries;
using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.Books.Queries.GetBooks;

public class GetBooksQuery : IQuery<Result<PaginationResult<BookResponse>>>
{
    public BooksQueryParameters QueryParameters { get; set; } = default!;
}
