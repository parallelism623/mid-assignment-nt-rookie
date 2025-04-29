
using MIDASS.Application.Commons.Models.Books;
using MIDASS.Domain.Entities;

namespace MIDASS.Application.Commons.Mapping;

public static class BookMapping
{
    public static BookDetailResponse ToBookDetailResponse(this Book book)
    {
        return new()
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Category = new BookCategoryResponse()
            {
                Name = book.Category.Name,
            }
        };
    }
    public static BookResponse ToBookResponse(this Book book)
    {
        return new()
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Category = new BookCategoryResponse()
            {
                Name = book.Category.Name,
            }
        };
    }
    public static List<BookDetailResponse> ToBookDetailResponses(this List<Book> books)
    {
        return books.Select(b => b.ToBookDetailResponse()).ToList();
    }
}
