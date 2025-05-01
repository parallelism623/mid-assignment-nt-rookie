
using MIDASM.Application.Commons.Models.Books;
using MIDASM.Domain.Entities;

namespace MIDASM.Application.Commons.Mapping;

public static class BookMapping
{
    public static BookDetailResponse ToBookDetailResponse(this Book book)
    {
        if(book == null)
        {
            return new();
        }
        return new()
        {
            Id = book.Id,
            Description = book.Description,
            Title = book.Title,
            Author = book.Author,
            Quantity = book.Quantity,
            Available = book.Available,
            ImageUrl = book.ImageUrl,
            SubImagesUrl = book.SubImagesUrl,
            Category = new BookCategoryResponse()
            {
                Id = book.Category.Id,
                Name = book.Category.Name
            },
            NumberOfReview = book.BookReviews?.Count() ?? 0,
            AverageRating = (!book.BookReviews?.Any() ?? true) ? 0 : Math.Round((decimal)book.BookReviews!.Sum(br => br.Rating) / book.BookReviews!.Count, 1)
        };
    }
    public static BookResponse ToBookResponse(this Book book)
    {
        if (book == null)
        {
            return new();
        }
        return new()
        {
            Id = book.Id,
            Description = book.Description,
            Title = book.Title,
            Author = book.Author,
            Quantity = book.Quantity,
            Available = book.Available,
            ImageUrl = book.ImageUrl,
            Category = new BookCategoryResponse()
            {
                Id = book.Category.Id,
                Name = book.Category.Name
            },
            NumberOfReview = book.BookReviews?.Count() ?? 0,
            AverageRating = (!book.BookReviews?.Any() ?? true) ? 0 : Math.Round((decimal)book.BookReviews!.Sum(br => br.Rating) / book.BookReviews!.Count, 1)
        };
    }
    public static List<BookDetailResponse> ToBookDetailResponses(this List<Book> books)
    {
        return books.Select(b => b.ToBookDetailResponse()).ToList();
    }
}
