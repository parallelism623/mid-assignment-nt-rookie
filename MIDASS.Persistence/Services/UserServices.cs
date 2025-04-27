
using Microsoft.EntityFrameworkCore;
using MIDASS.Application.Commons.Mapping;
using MIDASS.Application.Commons.Models;
using MIDASS.Application.Commons.Models.Users;
using MIDASS.Application.Services.Authentication;
using MIDASS.Application.UseCases;
using MIDASS.Contract.Errors;
using MIDASS.Contract.Messages.Commands;
using MIDASS.Contract.SharedKernel;
using MIDASS.Domain.Abstract;
using MIDASS.Domain.Entities;
using MIDASS.Domain.Repositories;
using MIDASS.Persistence.Specifications;

namespace MIDASS.Persistence.Services;

public class UserServices(IUserRepository userRepository, 
                            IBookBorrowingRequestRepository bookBorrowingRequestRepository,
                            IExecutionContext executionContext,
                            IBookRepository bookRepository,
                            ITransactionManager transactionManager) : IUserServices
{
    public async Task<Result<string>> CreateBookBorrowingRequestAsync(BookBorrowingRequestCreate bookBorrowingRequest)
    {
        
        var userId = executionContext.GetUserId();
        var user = await userRepository.GetByIdAsync(userId, "BookBorrowingRequests");

        if (user == null || user.Id != bookBorrowingRequest.RequesterId)
        {
            return Result<string>.Failure(400, UserErrors.UserCannotBeInCurrentSession);
        }

        if (user.BookBorrowingLimit == 0)
        {
            return Result<string>.Failure(400, UserErrors.UserReachBorrowingRequestLimit);
        }

        var booksIdRequest = bookBorrowingRequest.BorrowingRequestDetails.Select(bd => bd.BookId).ToList();
        var books = await bookRepository.GetByIdsAsync(booksIdRequest);

        if (books.Count != booksIdRequest.Count)
        {
            return Result<string>.Failure(400, UserErrors.UserBorrowingRequestBooksInvalid);
        }

        if (!CheckAvailableBooks(books))
        {
            return Result<string>.Failure(400, UserErrors.SomeBooksInBooksBorrowingRequestUnavailable);
        }

        await transactionManager.BeginTransactionAsync();

        try
        {
            foreach (var book in books)
            {
                book.Available -= 1;
            }

            bookRepository.UpdateRange(books);
            await bookRepository.SaveChangesAsync();

        }
        catch(DbUpdateConcurrencyException exception)
        {
       
            try
            {
            
                foreach (var entry in exception.Entries)
                {
                    if (entry.Entity is Book)
                    {
                        var databaseValues = await entry.GetDatabaseValuesAsync();
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                }

                books = await bookRepository.GetByIdsAsync(booksIdRequest);

                if (books.Count != booksIdRequest.Count)
                {
                    return Result<string>.Failure(400, UserErrors.UserBorrowingRequestBooksInvalid);
                }

                if (!CheckAvailableBooks(books))
                {
                    return Result<string>.Failure(400, UserErrors.SomeBooksInBooksBorrowingRequestUnavailable);
                }

                foreach (var book in books)
                {
                    book.Available -= 1;
                }

                bookRepository.UpdateRange(books);
                await bookRepository.SaveChangesAsync();
            }
            catch
            {
                await transactionManager.RollbackAsync();
                transactionManager.DisposeTransaction();
                return Result<string>.Failure(400, UserErrors.ErrorOccurWhenCreateBookBorrowingRequest);
            }
            
        }

        var booksBorrowingRequest = bookBorrowingRequest.ToBookBorrowingRequest();
        user.BookBorrowingLimit -= 1;
        if(user.BookBorrowingRequests is null)
        {
            user.BookBorrowingRequests = new List<BookBorrowingRequest>();
        }    
        user.BookBorrowingRequests.Add(booksBorrowingRequest);
        userRepository.Update(user);

        await userRepository.SaveChangesAsync();
        await transactionManager.CommitTransactionAsync();
        transactionManager.DisposeTransaction();
        return UserCommandMessages.BooksBorrowingRequestCreateSuccess;
    }

    public async Task<Result<PaginationResult<BookBorrowingRequestResponse>>> GetBookBorrowingRequestByIdAsync(Guid id, QueryParameters queryParameters)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return Result<PaginationResult<BookBorrowingRequestResponse>>.Failure(400, UserErrors.UserNotFound);
        }

        var query = bookBorrowingRequestRepository.GetQueryable();
        var querySpecification = new UserBookBorrowingRequestByQueryParameterSpecification(id, queryParameters);

        query = querySpecification.GetQuery(query);

        var totalCount = await query.CountAsync();

        var data = await query.Skip(queryParameters.PageSize * (queryParameters.PageIndex - 1))
            .Take(queryParameters.PageSize).Select(b => b.ToBookBorrowingRequestResponse()).ToListAsync();

        return PaginationResult<BookBorrowingRequestResponse>.Create(queryParameters.PageSize,
            queryParameters.PageIndex, totalCount, data);
    }

    private static bool CheckAvailableBooks(List<Book> books)
    {
        foreach (var book in books)
        {
            if (book.Available == 0)
                return false;
        }

        return true;
    }

    public async Task<Result<UserDetailResponse>> GetByIdAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id);

        return user?.ToUserDetailResponse() ?? default!; 
    }
}
