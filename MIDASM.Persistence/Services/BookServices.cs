using Mapster;
using Microsoft.EntityFrameworkCore;
using MIDASM.Application.Commons.Mapping;
using MIDASM.Application.Commons.Models.Books;
using MIDASM.Application.Services.FileServices;
using MIDASM.Application.UseCases;
using MIDASM.Contract.Errors;
using MIDASM.Contract.Messages.Commands;
using MIDASM.Contract.SharedKernel;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;
using MIDASM.Persistence.Specifications;

namespace MIDASM.Persistence.Services;

public class BookServices(IBookRepository bookRepository,
    ICategoryRepository categoryRepository,
    IBookReviewRepository bookReviewRepository,
    IImageStorageServices imageStorageServices) : IBookServices
{
    private const string DefaultBookImage = "default.jpeg";
    public async Task<Result<PaginationResult<BookResponse>>> GetsAsync(BooksQueryParameters queryParameters)
    {
        var queryableBook = bookRepository.GetQueryable();
        var queryableBookReview = bookReviewRepository.GetQueryable();
        var querySpecification = new BookByQueryParametersSpecification(queryParameters);
        queryableBook = querySpecification.GetQuery(queryableBook);


        var totalCount = await queryableBook.CountAsync();
        var query = queryableBook.Skip(queryParameters.PageSize * (queryParameters.PageIndex - 1))
            .Take(queryParameters.PageSize).GroupJoin(
            queryableBookReview,                          
            book => book.Id,          
            bookReview => bookReview.BookId,        
            (book, bookReviewGroup) => new BookResponse
            {
                Id = book.Id,
                Description = book.Description,
                Title  = book.Title,
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
                AverageRating = !bookReviewGroup.Any() ? 0 : Math.Round((decimal)bookReviewGroup.Sum(br => br.Rating)/bookReviewGroup.Count(), 1)
            }
        );



        var data = await query.ToListAsync();
        var tasks = data.Select(async b =>
        {
            if (string.IsNullOrEmpty(b.ImageUrl))
            {
                b.ImageUrl = DefaultBookImage;
            }
            b.ImageUrlSigned = await imageStorageServices.GetPreSignedUrlImage(b.ImageUrl);
        });

        await Task.WhenAll(tasks);
        return PaginationResult<BookResponse>.Create(queryParameters.PageSize,
            queryParameters.PageIndex, totalCount, data);
    }

    public async Task<Result<BookDetailResponse>> GetByIdAsync(Guid id)
    {
        var book = await bookRepository.GetByIdAsync(id, "Category", "BookReviews");
        var bookResponse = book!.ToBookDetailResponse();
        if(bookResponse.SubImagesUrl != null && bookResponse.SubImagesUrl.Any())
        {
            bookResponse.SubImagesUrlSigned = (await Task
                .WhenAll(bookResponse.SubImagesUrl.Select(GetSignedUrlSubImage)))
                .ToList();
        }
        if(string.IsNullOrEmpty(bookResponse.ImageUrl))
        {
            bookResponse.ImageUrl = DefaultBookImage;
        }
        bookResponse.ImageUrlSigned = await GetSignedUrlSubImage(bookResponse.ImageUrl);
        return bookResponse;
    }

    public async Task<Result<string>> CreateAsync(BookCreateRequest request)
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId);

        if (category == null)
        {
            return Result<string>.Failure(400, BookErrors.BookCanNotCreateDueToInvalidCategory);
        }
        List<string>? imagesSubUrl = new();
        if(request.SubImagesUrl != null && request.SubImagesUrl.Any())
        {
            imagesSubUrl = (await Task.WhenAll(request.SubImagesUrl.Select(b => imageStorageServices.UploadImageAsync(b))))!.ToList();
        }
        string? imageUrl = string.Empty;
        if(request.ImageUrl != null)
            imageUrl = await imageStorageServices.UploadImageAsync(request.ImageUrl);

        var book = Book.Create(request.Title, request.Description, request.Author, request.Quantity, request.Available,
            request.CategoryId, imageUrl, imagesSubUrl);

        bookRepository.Add(book);
        await bookRepository.SaveChangesAsync();

        return BookCommandMessages.BookCreatedSuccess;
    }

    public async Task<Result<string>> UpdateAsync(BookUpdateRequest request)
    {
        var book = await bookRepository.GetByIdAsync(request.Id, "Category");
        if (book == null)
        {
            return Result<string>.Failure(400, BookErrors.BookCanNotFound);
        }

        if (book.Available + request.AddedQuantity < 0)
        {
            return Result<string>.Failure(400, BookErrors.BookQuantityAddedInvalid);
        }

        if (book.Category.Id != request.CategoryId)
        {
            var category = await categoryRepository.GetByIdAsync(request.CategoryId);

            if (category == null)
            {
                return Result<string>.Failure(400, BookErrors.BookCanNotUpdateDueToInvalidCategory);
            }
        }

 
        Book.Update(book, request.Title, request.Description, request.Author, request.AddedQuantity,
            request.CategoryId);
        if(request.NewImage != null)
        {
            if(book.ImageUrl != null)
            {
                await imageStorageServices.DeleteImageAsync(book.ImageUrl);
            }
            var newImageUrl = await imageStorageServices.UploadImageAsync(request.NewImage);
            Book.UpdateImageUrl(book, newImageUrl);
        }
        if(request.SubImagesUrl != null && request.SubImagesUrl.Any())
        {
            var imageDeletedUrl = book.SubImagesUrl!.Where(i => !request.SubImagesUrl?.Contains(i) ?? true).ToList();
            await Task.WhenAll(imageDeletedUrl.Select(u => imageStorageServices.DeleteImageAsync(u)));
            Book.UpdateSubImagesUrl(book, request.SubImagesUrl);
        }   
        
        if (request.NewSubImages != null && request.NewSubImages.Any())
        {

            var newSubImagesUrl = (await Task.WhenAll(request.NewSubImages.Select(u => imageStorageServices.UploadImageAsync(u)))).ToList();

            var totalImage = (request.SubImagesUrl?.Count ?? 0) + (request.NewSubImages.Count);
            var newPositions = new HashSet<int>(request.NewSubImagesPos!);

            var finalSubImagesUrl = Enumerable
                .Range(0, totalImage)
                .Select(i =>
                {
                    var countNewBefore = request.NewSubImagesPos!
                        .Count(p => p < i);
                    return newPositions.Contains(i)
                        ? newSubImagesUrl[countNewBefore]
                        : book.SubImagesUrl![i - countNewBefore];
                })
                .ToList();
            Book.UpdateSubImagesUrl(book, finalSubImagesUrl);
        }
        bookRepository.Update(book);
        await bookRepository.SaveChangesAsync();
        return BookCommandMessages.BookUpdatedSuccess;
        
        
    }

    public async Task<Result<string>> DeleteAsync(Guid id)
    {
        var book = await bookRepository.GetByIdAsync(id, "BookBorrowingRequestDetails");
        if (book == null)
        {
            return Result<string>.Failure(400, BookErrors.BookCanNotFound);
        }

        if (book.BookBorrowingRequestDetails?.Count > 0)
        {
            return Result<string>.Failure(400, BookErrors.BookCanNotDeletedDueToExistsBorrowRequest);
        }
        book.IsDeleted = true;
        bookRepository.Update(book);
        await bookRepository.SaveChangesAsync();
        return BookCommandMessages.BookDeletedSuccess;
    }


    public async Task<Result<List<BookDetailResponse>>> GetByIdsAsync(string ids)
    {
        var bookIds = new List<Guid>();
        foreach(var bookId in ids.Split(','))
        {
            try
            {
                Guid.TryParse(bookId, out Guid id);
                bookIds.Add(id);
            }
            catch
            {
                return Result<List<BookDetailResponse>>.Failure(400, BookErrors.BookIdInvalid);
            }
        }
        var books = await bookRepository.GetByIdsAsync(bookIds);

        return books.ToBookDetailResponses();
    }

    public Task<string> GetSignedUrlSubImage(string subImageKey)
    {
        return imageStorageServices.GetPreSignedUrlImage(subImageKey);
    }

}
