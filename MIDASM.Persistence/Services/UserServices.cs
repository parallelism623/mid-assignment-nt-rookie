
using Microsoft.EntityFrameworkCore;
using MIDASM.Application.Commons.Mapping;
using MIDASM.Application.Commons.Models;
using MIDASM.Application.Commons.Models.BookBorrowingRequestDetails;
using MIDASM.Application.Commons.Models.Users;
using MIDASM.Application.Services.Authentication;
using MIDASM.Application.UseCases;
using MIDASM.Contract.Errors;
using MIDASM.Contract.Messages.Commands;
using MIDASM.Contract.SharedKernel;
using MIDASM.Domain.Abstract;
using MIDASM.Domain.Constrants;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Enums;
using MIDASM.Domain.Repositories;
using MIDASM.Persistence.Specifications;

namespace MIDASM.Persistence.Services;

public class UserServices(IUserRepository userRepository, 
                            IBookBorrowingRequestRepository bookBorrowingRequestRepository,
                            IExecutionContext executionContext,
                            IBookRepository bookRepository,
                            ITransactionManager transactionManager, 
                            IBookBorrowingRequestDetailRepository bookBorrowingRequestDetailRepository) : IUserServices
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

                var bookEntry = exception.Entries?.Where(entity => entity.Entity is Book)?.ToList();
                if(bookEntry != null)
                {
                    foreach (var entry in bookEntry)
                    {

                        var databaseValues = await entry.GetDatabaseValuesAsync();
                        if(databaseValues != null)
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

    public async Task<Result<PaginationResult<BookBorrowingRequestResponse>>> GetBookBorrowingRequestByIdAsync(Guid id, UserBookBorrowingRequestQueryParameters queryParameters)
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

    public async Task<Result<PaginationResult<BookBorrowedRequestDetailResponse>>> GetBookBorrowedRequestDetailByIdAsync(Guid id, QueryParameters queryParameters)
    {
        var userId = executionContext.GetUserId();
        if(userId != id)
        {
            return Result<PaginationResult<BookBorrowedRequestDetailResponse>>.Failure(400, UserErrors.UserCannotBeInCurrentSession);
        }
        var pageSize = queryParameters.PageSize;
        var pageIndex = queryParameters.PageIndex;
        var query = bookBorrowingRequestDetailRepository
            .GetQueryable()
            .Where(bd => bd.BookBorrowingRequest.RequesterId == id 
            && (bd.BookBorrowingRequest.Status == (int)BookBorrowingStatus.Approved));

        int totalCount = await query.CountAsync();

        var data = await query
                .OrderByDescending (b => b.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(bd => new BookBorrowedRequestDetailResponse()
                {
                    Id = bd.Id,
                    DueDate = bd.DueDate,
                    BookBorrowingRequestId = bd.BookBorrowingRequestId,
                    BookId = bd.BookId,
                    RequesterName = bd.BookBorrowingRequest.Requester.FirstName
                                    + bd.BookBorrowingRequest.Requester.LastName,
                    ApproverName = bd.BookBorrowingRequest == null
                    ? default
                    : bd.BookBorrowingRequest.Approver!.FirstName + bd.BookBorrowingRequest.Approver.LastName,
                    Book = new Application.Commons.Models.Books.BookResponse
                    {
                        Id = bd.Book.Id,
                        Title = bd.Book.Title,
                        Author = bd.Book.Author,
                        Category = new()
                        {
                            Id = bd.Book.Category.Id,   
                            Name = bd.Book.Category.Name
                        }
                    },
                    Noted = bd.Noted,
                    ExtendDueDateTimes = bd.ExtendDueDateTimes,
                    ExtendDueDate = bd.ExtendDueDate,
                })
                .ToListAsync();


        return PaginationResult<BookBorrowedRequestDetailResponse>.Create(pageSize, pageIndex, totalCount, data);

    }

    public async Task<Result<string>> ExtendDueDateBookBorrowed(DueDatedExtendRequest dueDatedExtendRequest)
    {
        var bookBorrowedDetail = await bookBorrowingRequestDetailRepository
                                    .GetByIdAsync(dueDatedExtendRequest.BookBorrowedDetailId, "BookBorrowingRequest");
        if(bookBorrowedDetail == null)
        {
            return Result<string>.Failure(400, UserErrors.BookBorrowedNotExistsCanNotExtendDueDate);
        }

        if(bookBorrowedDetail.BookBorrowingRequest.Status == (int)BookBorrowingStatus.Rejected)
        {
            return Result<string>.Failure(400, UserErrors.BookBorrowRejectCanNotExtendDueDate);
        }

        if(bookBorrowedDetail.ExtendDueDateTimes == BookBorrowingRequestDetailValidationRules.MaxExtendDueDateTimes)
        {
            return Result<string>.Failure(400, UserErrors.BookBorrowedExtendDueDateTimesReachLimit);
        }

        if(bookBorrowedDetail.DueDate > dueDatedExtendRequest.ExtendDueDate)
        {
            return Result<string>.Failure(400, UserErrors.BookBorrowedNewExtendDueDateInValid);
        }

        bookBorrowedDetail.ExtendDueDate = dueDatedExtendRequest.ExtendDueDate;
        bookBorrowedDetail.ExtendDueDateTimes += 1;

        bookBorrowingRequestDetailRepository.Update(bookBorrowedDetail);
        await bookBorrowingRequestDetailRepository.SaveChangesAsync();

        return UserCommandMessages.BookBorrowedExtendDueDateSuccess;
    }
}
