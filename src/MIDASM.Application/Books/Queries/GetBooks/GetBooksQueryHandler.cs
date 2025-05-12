
using Microsoft.Extensions.Configuration;
using MIDASM.Application.Commons.Models.Books;
using MIDASM.Application.Dispatcher.Queries;
using MIDASM.Application.Services.FileServices;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;
using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.Books.Queries.GetBooks;

public class GetBooksQueryHandler(
    IBookRepository bookRepository, 
    IBookReviewRepository bookReviewRepository,
    IConfiguration config,
    IImageStorageServices imageStorageServices)
    : IQueryHandler<GetBooksQuery, Result<PaginationResult<BookResponse>>>
{
    public async Task<Result<PaginationResult<BookResponse>>> HandlerAsync(GetBooksQuery request, CancellationToken cancellationToken = default)
    {
        var queryParameters = request.QueryParameters;
        var queryableBook = bookRepository.GetQueryable();
        var queryableBookReview = bookReviewRepository.GetQueryable();
        queryableBook = ApplyQueryParameters(queryableBook, queryParameters);


        var totalCount = await bookRepository.CountAsync(queryableBook);
        var query = queryableBook.Skip(queryParameters.PageSize * (queryParameters.PageIndex - 1))
            .Take(queryParameters.PageSize).GroupJoin(
                queryableBookReview,
                book => book.Id,
                bookReview => bookReview.BookId,
                (book, bookReviewGroup) => new BookResponse
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
                    NumberOfReview = bookReviewGroup.Count(),
                    AverageRating = !bookReviewGroup.Any() ? 0 : Math.Round((decimal)bookReviewGroup.Sum(br => br.Rating) / bookReviewGroup.Count(), 1)
                }
        );



        var data = await bookRepository.ToListAsync(query);
        var tasks = data.Select(async b =>
        {
            if (string.IsNullOrEmpty(b.ImageUrl))
            {
                b.ImageUrl = config.GetRequiredSection("Default:DefaultBookImage").Value 
                    ?? throw new ArgumentException("Book image default does not exists");
            }
            b.ImageUrlSigned = await imageStorageServices.GetPreSignedUrlImage(b.ImageUrl);
        });

        await Task.WhenAll(tasks);
        return PaginationResult<BookResponse>.Create(queryParameters.PageSize,
            queryParameters.PageIndex, totalCount, data);
    }
    public static IQueryable<Book> ApplyQueryParameters(IQueryable<Book> query,
        BooksQueryParameters p)
    {
        if (p.Availability)
        {
            query = query.Where(b => b.Available > 0);
        }
        if (!string.IsNullOrEmpty(p.Search))
        {
            query = query.Where(b =>
                b.Title.Contains(p.Search) ||
                b.Author.Contains(p.Search) ||
                (b.Category != null && b.Category.Name.Contains(p.Search)));
        }
        if (p.Ids != null && p.Ids.Any())
        {
            query = query.Where(b => p.Ids.Contains(b.Id));
        }

        if (p.CategoryIds != null && p.CategoryIds.Any())
        {
            query = query.Where(b => p.CategoryIds.Contains(b.CategoryId));
        }
        query = query.Where(b =>
            (b.BookReviews != null && b.BookReviews.Any()
                ? b.BookReviews.Average(br => br.Rating)
                : 0)
            >= p.Rating);

        return query
            .OrderByDescending(b => b.CreatedAt);
    }
}
