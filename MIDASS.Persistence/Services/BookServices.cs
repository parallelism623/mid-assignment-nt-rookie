using Mapster;
using Microsoft.EntityFrameworkCore;
using MIDASS.Application.Commons.Models.Books;
using MIDASS.Application.UseCases;
using MIDASS.Contract.Errors;
using MIDASS.Contract.Messages.Commands;
using MIDASS.Contract.SharedKernel;
using MIDASS.Domain.Entities;
using MIDASS.Domain.Repositories;
using MIDASS.Persistence.Specifications;
using static System.Reflection.Metadata.BlobBuilder;
using System.Transactions;
using MIDASS.Application.Commons.Mapping;

namespace MIDASS.Persistence.Services;

public class BookServices(IBookRepository bookRepository,
    ICategoryRepository categoryRepository) : IBookServices
{
    public async Task<Result<PaginationResult<BookResponse>>> GetsAsync(BooksQueryParameters queryParameters)
    {
        var query = bookRepository.GetQueryable();
        var querySpecification = new BookByQueryParametersSpecification(queryParameters);

        query = querySpecification.GetQuery(query);

        var totalCount = await query.CountAsync();

        var data = await query.Skip(queryParameters.PageSize * (queryParameters.PageIndex - 1))
            .Take(queryParameters.PageSize).ToListAsync();
        return PaginationResult<BookResponse>.Create(queryParameters.PageSize,
            queryParameters.PageIndex, totalCount, GetBookResponses(data));
    }

    public async Task<Result<BookDetailResponse>> GetByIdAsync(Guid id)
    {
        var book = await bookRepository.GetByIdAsync(id, "Category");
        var bookResponse = new BookDetailResponse();

        if (book != null)
        {
            bookResponse = book.Adapt<BookDetailResponse>();
            bookResponse.Category = book.Category.Adapt<BookCategoryResponse>();
        }

        return bookResponse;
    }

    public async Task<Result<string>> CreateAsync(BookCreateRequest request)
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId);

        if (category == null)
        {
            return Result<string>.Failure(400, BookErrors.BookCanNotCreateDueToInvalidCategory);
        }

        var book = Book.Create(request.Title, request.Description, request.Author, request.Quantity, request.Available,
            request.CategoryId);

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

        try
        {
            Book.Update(book, request.Title, request.Description, request.Author, request.AddedQuantity,
                request.CategoryId);
            bookRepository.Update(book);
            await bookRepository.SaveChangesAsync();
            return BookCommandMessages.BookUpdatedSuccess;
        }
        catch (DbUpdateConcurrencyException exception)
        {
            try
            {

                foreach (var entry in exception.Entries)
                {
                    if (entry.Entity is Book)
                    {
                        var dbValues = await entry.GetDatabaseValuesAsync();
                        entry.OriginalValues.SetValues(dbValues);
                    }
                }
                book = await bookRepository.GetByIdAsync(request.Id, "Category");
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
                bookRepository.Update(book);
                await bookRepository.SaveChangesAsync();
                return BookCommandMessages.BookUpdatedSuccess;

            }
            catch
            {
                return Result<string>.Failure(400, UserErrors.ErrorOccurWhenUpdateBookBorrowingRequest);
            }
        }

        
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

    private static List<BookResponse> GetBookResponses(List<Book> books)
    {
        return books.Select(b =>
        {
            var bookResponse = b.Adapt<BookResponse>();
            bookResponse.Category = b.Category.Adapt<BookCategoryResponse>();
            return bookResponse;
        }).ToList();
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

}
