using Mapster;
using Microsoft.EntityFrameworkCore;
using MIDASM.Application.Commons.Mapping;
using MIDASM.Application.Commons.Models.Books;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.Services.Authentication;
using MIDASM.Application.Services.FileServices;
using MIDASM.Application.UseCases;
using MIDASM.Contract.Errors;
using MIDASM.Contract.Helpers;
using MIDASM.Contract.Messages.AuditLogMessage;
using MIDASM.Contract.Messages.Commands;
using MIDASM.Contract.SharedKernel;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Repositories;
using MIDASM.Persistence.Specifications;

namespace MIDASM.Persistence.UseCases;

public class BookServices(IBookRepository bookRepository,
    ICategoryRepository categoryRepository,
    IBookReviewRepository bookReviewRepository,
    IImageStorageServices imageStorageServices, 
    IAuditLogger auditLogger,
    IExecutionContext executionContext) : IBookServices
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
        var book = await bookRepository
            .GetByIdAsync(id,
            nameof(Book.Category),
            nameof(Book.BookReviews));

        var bookResponse = book!.ToBookDetailResponse();

        if(IsBookHasSubImages(bookResponse))
        {
            await GetBookSignedSubImagesAsync(bookResponse);
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
        List<string> imagesSubUrl = new();
        if(request.SubImagesUrl != null && request.SubImagesUrl.Count != 0)
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
        await HandleAuditLogBookCreate(book);
        return BookCommandMessages.BookCreatedSuccess;
    }

    public async Task<Result<string>> UpdateAsync(BookUpdateRequest request)
    {
        var book = await bookRepository.GetByIdAsync(request.Id, nameof(Book.Category));
        if (book == null)
        {
            return Result<string>.Failure(BookErrors.BookCanNotFound);
        }

        if (book.Available + request.AddedQuantity < 0)
        {
            return Result<string>.Failure(BookErrors.BookQuantityAddedInvalid);
        }

        if (book.Category.Id != request.CategoryId)
        {
            var category = await categoryRepository.GetByIdAsync(request.CategoryId);

            if (category == null)
            {
                return Result<string>.Failure(BookErrors.BookCanNotUpdateDueToInvalidCategory);
            }
        }

        var oldBook = Book.Copy(book);
        Book.Update(book, request.Title, request.Description, request.Author, request.AddedQuantity,
            request.CategoryId);
        if(request.ImageUrl == null)
        {
            if(book.ImageUrl != null && book.ImageUrl != DefaultBookImage)
            {
                await imageStorageServices.DeleteImageAsync(book.ImageUrl);
            }

            Book.UpdateImageUrl(book, DefaultBookImage);
            if (request.NewImage != null)
            {
                var newImageUrl = await imageStorageServices.UploadImageAsync(request.NewImage);
                Book.UpdateImageUrl(book, newImageUrl);
            }
        }
    
        var imageDeletedUrl = book.SubImagesUrl?.Where(i => !request.SubImagesUrl?.Contains(i) ?? true)?.ToList();
        if (imageDeletedUrl != null && imageDeletedUrl.Count > 0)
        {
            await Task.WhenAll(imageDeletedUrl.Select(u => imageStorageServices.DeleteImageAsync(u)));
            Book.UpdateSubImagesUrl(book, request.SubImagesUrl!);
        }
          
        
        if (request.NewSubImages != null && request.NewSubImages.Count != 0)
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

        await HandleAuditLogBookUpdate(book, oldBook);
        return BookCommandMessages.BookUpdatedSuccess;
        
        
    }

    public async Task<Result<string>> DeleteAsync(Guid id)
    {
        var book = await bookRepository.GetByIdAsync(id, nameof(Book.BookBorrowingRequestDetails));
        if (book == null)
        {
            return Result<string>.Failure(400, BookErrors.BookCanNotFound);
        }

        if (book.BookBorrowingRequestDetails?.Count > 0)
        {
            return Result<string>.Failure(400, BookErrors.BookCanNotDeletedDueToExistsBorrowRequest);
        }
        var oldBook = Book.Copy(book);
        book.IsDeleted = true;
        bookRepository.Update(book);
        await bookRepository.SaveChangesAsync();
        await HandleAuditLogBookDelete(book, oldBook);
        return BookCommandMessages.BookDeletedSuccess;
    }


    public async Task<Result<List<BookDetailResponse>>> GetByIdsAsync(string ids)
    {
        var bookIds = new List<Guid>();
        var segments = ids.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var segment in segments)
        {
            if (!Guid.TryParse(segment.Trim(), out var id))
            {
                return Result<List<BookDetailResponse>>
                    .Failure(400, BookErrors.BookIdInvalid);
            }

            bookIds.Add(id);
        }

        var books = await bookRepository.GetByIdsAsync(bookIds);
        return books.ToBookDetailResponses();
    }

    public Task<string> GetSignedUrlSubImage(string subImageKey)
    {
        return imageStorageServices.GetPreSignedUrlImage(subImageKey);
    }

    public async Task HandleAuditLogBookCreate(Book newBook)
    {
        var propertiesChanged = GetChangedProperties(newBook);

        await auditLogger.LogAsync(
                newBook.Id.ToString(),
                nameof(Book),
                StringHelper.ReplacePlaceholders(
                    AuditLogMessageTemplate.Create,
                    executionContext.GetUserName(),
                    nameof(Book).ToLower(),
                    $"{newBook.Title} (#{newBook.Id})",
                    newBook.CreatedAt.ToShortTime()
                    ),
                propertiesChanged
            );
    }
    public async Task HandleAuditLogBookUpdate(Book newBook, Book oldBook)
    {
        var propertiesChanged = GetChangedProperties(newBook, oldBook);

        await auditLogger.LogAsync(
                newBook.Id.ToString(),
                nameof(Book),
                StringHelper.ReplacePlaceholders(
                    AuditLogMessageTemplate.Update,
                    executionContext.GetUserName(),
                    nameof(Book).ToLower(),
                    $"{newBook.Title} (#{newBook.Id})",
                    newBook.ModifiedAt!.ToShortTime(),
                    StringHelper.SerializePropertiesChanges(propertiesChanged)
                    ),
                propertiesChanged
            );
    }

    public async Task HandleAuditLogBookDelete(Book newBook, Book oldBook)
    {
        var propertiesChanged = GetChangedProperties(newBook, oldBook);

        await auditLogger.LogAsync(
                newBook.Id.ToString(),
                nameof(Book),
                StringHelper.ReplacePlaceholders(
                    AuditLogMessageTemplate.Delete,
                    executionContext.GetUserName(),
                    nameof(Book).ToLower() ,
                    $"{newBook.Title} (#{newBook.Id})",
                    newBook.ModifiedAt!.ToShortTime()
                    ),
                propertiesChanged
            );
    }

    private static Dictionary<string, (string?, string?)> GetChangedProperties(Book newBook, Book? oldBook = default)
    {
        var changes = new Dictionary<string, (string?, string?)>();

        if (oldBook == null || newBook.Title != oldBook.Title)
            changes.Add(nameof(newBook.Title), (oldBook?.Title, newBook.Title));

        if (oldBook == null || newBook.Description != oldBook.Description)
            changes.Add(nameof(newBook.Description), (oldBook?.Description, newBook.Description));

        if (oldBook == null || newBook.Author != oldBook.Author)
            changes.Add(nameof(newBook.Author), (oldBook?.Author, newBook.Author));

        if (oldBook == null || newBook.Quantity != oldBook.Quantity)
            changes.Add(nameof(newBook.Quantity), (oldBook?.Quantity.ToString(), newBook.Quantity.ToString()));

        if (oldBook == null || newBook.Available != oldBook.Available)
            changes.Add(nameof(newBook.Available), (oldBook?.Available.ToString(), newBook.Available.ToString()));

        if (oldBook == null || newBook.ImageUrl != oldBook.ImageUrl)
            changes.Add(nameof(newBook.ImageUrl), (oldBook?.ImageUrl, newBook.ImageUrl));

        if (oldBook == null ||
            string.Join(";", newBook.SubImagesUrl ?? new List<string>()) != string.Join(";", oldBook?.SubImagesUrl ?? new List<string>()))
        {
            changes.Add(nameof(newBook.SubImagesUrl), (
                oldBook?.SubImagesUrl != null ? string.Join(";", oldBook.SubImagesUrl) : null,
                newBook.SubImagesUrl != null ? string.Join(";", newBook.SubImagesUrl) : null
            ));
        }

        if (oldBook == null || newBook.CategoryId != oldBook.CategoryId)
            changes.Add(nameof(newBook.CategoryId), (oldBook?.CategoryId.ToString(), newBook.CategoryId.ToString()));


        return changes;
    }


    private static bool IsBookHasSubImages(BookDetailResponse bookResponse)
    {
        return bookResponse.SubImagesUrl != null && bookResponse.SubImagesUrl.Count != 0;
    }

    private async Task GetBookSignedSubImagesAsync(BookDetailResponse bookResponse)
    {
        if (bookResponse.SubImagesUrl != null)
        {
            bookResponse.SubImagesUrlSigned = (await Task
                    .WhenAll(bookResponse.SubImagesUrl
                        .Select(GetSignedUrlSubImage)))
                .ToList();
        }
    }
}
