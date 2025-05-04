
using Microsoft.EntityFrameworkCore;
using MIDASM.Application.Commons.Mapping;
using MIDASM.Application.Commons.Models;
using MIDASM.Application.Commons.Models.BookBorrowingRequestDetails;
using MIDASM.Application.Commons.Models.Users;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.Services.Authentication;
using MIDASM.Application.Services.Crypto;
using MIDASM.Application.UseCases;
using MIDASM.Contract.Errors;
using MIDASM.Contract.Helpers;
using MIDASM.Contract.Messages.AuditLogMessage;
using MIDASM.Contract.Messages.Commands;
using MIDASM.Contract.SharedKernel;
using MIDASM.Domain.Abstract;
using MIDASM.Domain.Constrants.Validations;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Enums;
using MIDASM.Domain.Repositories;
using MIDASM.Infrastructure.Crypto;
using MIDASM.Persistence.Specifications;

namespace MIDASM.Persistence.Services;

public class UserServices(IUserRepository userRepository, 
                            IBookBorrowingRequestRepository bookBorrowingRequestRepository,
                            IExecutionContext executionContext,
                            IBookRepository bookRepository,
                            ITransactionManager transactionManager, 
                            IBookBorrowingRequestDetailRepository bookBorrowingRequestDetailRepository,
                            IRoleRepository roleRepository, 
                            ICryptoServiceFactory cryptoServiceFactory, 
                            IAuditLogger auditLogger) : IUserServices
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
        await HandleAuditLogCreateBookBorrowingRequest(booksBorrowingRequest);
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
                                    .GetByIdAsync(dueDatedExtendRequest.BookBorrowedDetailId, "BookBorrowingRequest", "Book");
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
        var oldBookBorrowedDetail = BookBorrowingRequestDetail.Copy(bookBorrowedDetail);

        bookBorrowedDetail.ExtendDueDate = dueDatedExtendRequest.ExtendDueDate;
        bookBorrowedDetail.ExtendDueDateTimes += 1;

        bookBorrowingRequestDetailRepository.Update(bookBorrowedDetail);
        await bookBorrowingRequestDetailRepository.SaveChangesAsync();
        await HandleAuditLogCreateExtendDueDate(bookBorrowedDetail, oldBookBorrowedDetail);
        return UserCommandMessages.BookBorrowedExtendDueDateSuccess;
    }

    public async Task<Result<PaginationResult<UserDetailResponse>>> GetAsync(UserQueryParameters queryParameters)
    {
        var query = userRepository.GetQueryable();
        var querySpecification = new UserByQueryParametersSpecification(queryParameters);

        query = querySpecification.GetQuery(query);

        var totalCount = await query.CountAsync();

        var pageSize = queryParameters.PageSize;
        var pageIndex = queryParameters.PageIndex;

        var data = await query.Skip(pageSize * (pageIndex - 1))
                              .Take(pageSize).ToListAsync();

        var userData = data.Select(d => d.ToUserDetailResponse()).ToList();

        return PaginationResult<UserDetailResponse>.Create(pageSize, pageIndex, totalCount, userData);
    }

    public async Task<Result<string>> UpdateProfileAsync(Guid id, UserProfileUpdateRequest userProfileUpdateRequest)
    {
        var user = await userRepository.GetByIdAsync(id);
        var userId = executionContext.GetUserId();
        if(userId != id)
        {
            return Result<string>.Failure(400, UserErrors.UserCannotBeInCurrentSession);
        }
        if (user == null)
        {
            return Result<string>.Failure(400, UserErrors.UserNotFound);
        }

        User.UpdateProfile(user, userProfileUpdateRequest.FirstName, userProfileUpdateRequest.LastName, userProfileUpdateRequest.PhoneNumber);
        await userRepository.SaveChangesAsync();

        return UserCommandMessages.UserUpdateSuccessfully;
    }


    public async Task<Result<string>> UpdateAsync(UserUpdateRequest updateRequest)
    {
        var user = await userRepository.GetByIdAsync(updateRequest.Id);

        if(user == null)
        {
            return Result<string>.Failure(400, UserErrors.UserNotFound);
        }

        var role = await roleRepository.GetByIdAsync(updateRequest.RoleId);

        if(role == null)
        {
            return Result<string>.Failure(400, UserErrors.UserRoleNotFound);
        }

        if(!string.IsNullOrEmpty(updateRequest.Password))
        {
            var crtyptoSerivce = cryptoServiceFactory.SetCryptoAlgorithm("RSA");
            updateRequest.Password = crtyptoSerivce.Encrypt(updateRequest.Password);    
        }
        var oldUser = User.Copy(user);

        User.Update(user, updateRequest.Email, updateRequest.Password, updateRequest.FirstName, updateRequest.LastName,
            updateRequest.PhoneNumber, updateRequest.BookBorrowingLimit, updateRequest.RoleId);

        await userRepository.SaveChangesAsync();

        await HandleAuditLogUserUpdate(user, oldUser);
        return UserCommandMessages.UserUpdateSuccessfully;
    }

    public async Task<Result<string>> CreateAsync(UserCreateRequest createRequest)
    {
        var userByUserName = await userRepository.GetByUsernameAsync(createRequest.Username);

        if (userByUserName != null)
        {
            return Result<string>.Failure(400, UserErrors.UsernameAlreadyExists);
        }

        var userByEmail = await userRepository.GetByEmailAsync(createRequest.Email);

        if (userByEmail != null)
        {
            return Result<string>.Failure(400, UserErrors.EmailAlreadyExists);
        }

        var role = await roleRepository.GetByIdAsync(createRequest.RoleId);

        if (role == null)
        {
            return Result<string>.Failure(400, UserErrors.UserRoleNotFound);
        }

        var cryptoSerivce = cryptoServiceFactory.SetCryptoAlgorithm("RSA");
        var user = User.Create(createRequest.Email, createRequest.Username, cryptoSerivce.Encrypt(createRequest.Password),
            createRequest.FirstName, createRequest.LastName, createRequest.PhoneNumber, createRequest.RoleId, true);
       
        userRepository.Add(user);
        await userRepository.SaveChangesAsync();
        await HandleAuditLogUserCreate(user);
        return UserCommandMessages.UserCreateSuccessfully;
    }

    public async Task<Result<string>> DeleteAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id);

        if (user == null)
        {
            return Result<string>.Failure(400, UserErrors.UserNotFound);
        }
        var oldUser = User.Copy(user);
        User.Delete(user);
        await userRepository.SaveChangesAsync();
        await HandleAuditLogUserDelete(user, oldUser);
        return UserCommandMessages.UserDeleteSuccessfully;
    }

    private async Task HandleAuditLogUserCreate(User user)
    {
        var propertiesChanged = GetChangedUserProperties(user);
        await auditLogger.LogAsync(user.Id.ToString(),
            nameof(User),
            StringHelper.ReplacePlaceholders(
                AuditLogMessageTemplate.Create,
                executionContext.GetUserName(),
                nameof(User).ToLower(),
                $"{user.Username} (#ID: {user.Id})",
                user.CreatedAt.ToString()
                ), propertiesChanged);
    }
    private async Task HandleAuditLogUserUpdate(User user, User oldUser)
    {
        var propertiesChanged = GetChangedUserProperties(user, oldUser);
        await auditLogger.LogAsync(user.Id.ToString(),
            nameof(User),
            StringHelper.ReplacePlaceholders(
                AuditLogMessageTemplate.Update,
                executionContext.GetUserName(),
                nameof(User).ToLower(),
                $"{user.Username} (#ID: {user.Id})",
                user.CreatedAt.ToString(),
                StringHelper.SerializePropertiesChanges(propertiesChanged)
                ), propertiesChanged);  
    }
    private async Task HandleAuditLogCreateBookBorrowingRequest(BookBorrowingRequest bookBorrowingRequest)
    {
        await auditLogger.LogAsync(
            bookBorrowingRequest.Id.ToString(),
            nameof(BookBorrowingRequest),
            StringHelper.ReplacePlaceholders(
                AuditLogMessageTemplate.CreateBookBorrowingRequest,
                executionContext.GetUserName(),
                bookBorrowingRequest.Id.ToString(),
                bookBorrowingRequest.CreatedAt.ToString()
                ),
            GetChangedBookBorrowingRequestProperties(bookBorrowingRequest));
    }


    private async Task HandleAuditLogCreateExtendDueDate(BookBorrowingRequestDetail newDetail, BookBorrowingRequestDetail oldDetail)
    {
        var propertyChanged = GetChangedBookBorrowingRequestDetailProperties(newDetail, oldDetail);
        await auditLogger.LogAsync(newDetail.Id.ToString(),
            nameof(BookBorrowingRequestDetail),
            StringHelper.ReplacePlaceholders(
                AuditLogMessageTemplate.ExtendDueDate,
                executionContext.GetUserName(),
                newDetail.Book.Title,
                newDetail.DueDate.ToString(),
                newDetail.ExtendDueDate?.ToString() ?? string.Empty,
                newDetail.ModifiedAt?.ToString() ?? string.Empty
                ), propertyChanged);
    }

    private async Task HandleAuditLogUserDelete(User user, User oldUser)
    {
        var propertiesChanged = GetChangedUserProperties(user, oldUser);
        await auditLogger.LogAsync(
            user.Id.ToString(),
            nameof(User),
            StringHelper.ReplacePlaceholders(
                AuditLogMessageTemplate.Delete,
                executionContext.GetUserName(),
                nameof(User).ToLower(),
                $"{user.Username} (#ID: {user.Id})",
                user.ModifiedAt?.ToString() ?? string.Empty
                ),
            propertiesChanged);
    }
    private static Dictionary<string, (string?, string?)> GetChangedBookBorrowingRequestProperties(
BookBorrowingRequest newRequest, BookBorrowingRequest? oldRequest = default)
    {
        var changes = new Dictionary<string, (string?, string?)>();

        if (oldRequest == null || newRequest.RequesterId != oldRequest.RequesterId)
            changes.Add(nameof(newRequest.RequesterId), (oldRequest?.RequesterId.ToString(), newRequest.RequesterId.ToString()));

        if (oldRequest == null || newRequest.ApproverId != oldRequest.ApproverId)
            changes.Add(nameof(newRequest.ApproverId), (oldRequest?.ApproverId?.ToString(), newRequest.ApproverId?.ToString()));

        if (oldRequest == null || newRequest.DateRequested != oldRequest.DateRequested)
            changes.Add(nameof(newRequest.DateRequested), (oldRequest?.DateRequested.ToString(), newRequest.DateRequested.ToString()));

        if (oldRequest == null || newRequest.DateApproved != oldRequest.DateApproved)
            changes.Add(nameof(newRequest.DateApproved), (oldRequest?.DateApproved?.ToString(), newRequest.DateApproved?.ToString()));

        if (oldRequest == null || newRequest.Status != oldRequest.Status)
            changes.Add(nameof(newRequest.Status), (oldRequest?.Status.ToString(), newRequest.Status.ToString()));

        if (oldRequest == null || newRequest.IsDeleted != oldRequest.IsDeleted)
            changes.Add(nameof(newRequest.IsDeleted), (oldRequest?.IsDeleted.ToString(), newRequest.IsDeleted.ToString()));

        return changes;
    }
    public static Dictionary<string, (string? OldValue, string? NewValue)> GetChangedUserProperties(User newUser, User? oldUser = default)
    {
        var changes = new Dictionary<string, (string?, string?)>();

        if (oldUser == null || newUser.Username != oldUser.Username)
            changes[nameof(newUser.Username)] = (oldUser?.Username, newUser.Username);

        if (oldUser == null || newUser.Password != oldUser.Password)
            changes[nameof(newUser.Password)] = (oldUser?.Password, newUser.Password);

        if (oldUser == null || newUser.Email != oldUser.Email)
            changes[nameof(newUser.Email)] = (oldUser?.Email, newUser.Email);

        if (oldUser == null || newUser.FirstName != oldUser.FirstName)
            changes[nameof(newUser.FirstName)] = (oldUser?.FirstName, newUser.FirstName);

        if (oldUser == null || newUser.LastName != oldUser.LastName)
            changes[nameof(newUser.LastName)] = (oldUser?.LastName, newUser.LastName);

        if (oldUser == null || newUser.PhoneNumber != oldUser.PhoneNumber)
            changes[nameof(newUser.PhoneNumber)] = (oldUser?.PhoneNumber, newUser.PhoneNumber);

        if (oldUser == null || newUser.IsDeleted != oldUser.IsDeleted)
            changes[nameof(newUser.IsDeleted)] = (oldUser?.IsDeleted.ToString(), newUser.IsDeleted.ToString());

        if (oldUser == null || newUser.BookBorrowingLimit != oldUser.BookBorrowingLimit)
            changes[nameof(newUser.BookBorrowingLimit)] = (oldUser?.BookBorrowingLimit.ToString(), newUser.BookBorrowingLimit.ToString());

        if (oldUser == null || newUser.LastUpdateLimit != oldUser.LastUpdateLimit)
            changes[nameof(newUser.LastUpdateLimit)] = (oldUser?.LastUpdateLimit.ToString(), newUser.LastUpdateLimit.ToString());

        if (oldUser == null || newUser.RefreshToken != oldUser.RefreshToken)
            changes[nameof(newUser.RefreshToken)] = (oldUser?.RefreshToken, newUser.RefreshToken);

        if (oldUser == null || newUser.RefreshTokenExpireTime != oldUser.RefreshTokenExpireTime)
            changes[nameof(newUser.RefreshTokenExpireTime)] = (oldUser?.RefreshTokenExpireTime.ToString(), newUser.RefreshTokenExpireTime.ToString());

        if (oldUser == null || newUser.IsVerifyCode != oldUser.IsVerifyCode)
            changes[nameof(newUser.IsVerifyCode)] = (oldUser?.IsVerifyCode.ToString(), newUser.IsVerifyCode.ToString());

        if (oldUser == null || newUser.RoleId != oldUser.RoleId)
            changes[nameof(newUser.RoleId)] = (oldUser?.RoleId.ToString(), newUser.RoleId.ToString());

        return changes;
    }
    private static Dictionary<string, (string? OldValue, string? NewValue)> GetChangedBookBorrowingRequestDetailProperties(
BookBorrowingRequestDetail newDetail, BookBorrowingRequestDetail? oldDetail = default)
    {
        var changes = new Dictionary<string, (string?, string?)>();

        if (oldDetail == null || newDetail.BookBorrowingRequestId != oldDetail.BookBorrowingRequestId)
            changes[nameof(newDetail.BookBorrowingRequestId)] = (oldDetail?.BookBorrowingRequestId.ToString(), newDetail.BookBorrowingRequestId.ToString());

        if (oldDetail == null || newDetail.BookId != oldDetail.BookId)
            changes[nameof(newDetail.BookId)] = (oldDetail?.BookId.ToString(), newDetail.BookId.ToString());

        if (oldDetail == null || newDetail.DueDate != oldDetail.DueDate)
            changes[nameof(newDetail.DueDate)] = (oldDetail?.DueDate.ToString(), newDetail.DueDate.ToString());

        if (oldDetail == null || newDetail.IsDeleted != oldDetail.IsDeleted)
            changes[nameof(newDetail.IsDeleted)] = (oldDetail?.IsDeleted.ToString(), newDetail.IsDeleted.ToString());

        if (oldDetail == null || newDetail.Noted != oldDetail.Noted)
            changes[nameof(newDetail.Noted)] = (oldDetail?.Noted, newDetail.Noted);

        if (oldDetail == null || newDetail.ExtendDueDateTimes != oldDetail.ExtendDueDateTimes)
            changes[nameof(newDetail.ExtendDueDateTimes)] = (oldDetail?.ExtendDueDateTimes.ToString(), newDetail.ExtendDueDateTimes.ToString());

        if (oldDetail == null || newDetail.ExtendDueDate != oldDetail.ExtendDueDate)
            changes[nameof(newDetail.ExtendDueDate)] = (oldDetail?.ExtendDueDate?.ToString(), newDetail.ExtendDueDate?.ToString());

        return changes;
    }

}
