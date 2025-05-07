using Microsoft.EntityFrameworkCore;
using MIDASM.Application.Commons.Models.Auditlogs;
using MIDASM.Application.Commons.Models.Report;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.UseCases;
using MIDASM.Contract.SharedKernel;
using MIDASM.Domain.Enums;
using MIDASM.Domain.Repositories;

namespace MIDASM.Persistence.UseCases;

public class ReportServices(IBookRepository bookRepository, 
        IBookBorrowingRequestDetailRepository bookBorrowingRequestDetailRepository, 
        IUserRepository userRepository,
        IAuditLogger auditLogger,
        ICategoryRepository categoryRepository, 
        IBookBorrowingRequestRepository bookBorrowingRequestRepository) : IReportServices
{
    public async Task<Result<PaginationResult<BookBorrowingReportResponse>>> GetBookBorrowingReportAsync(BookBorrowingReportQueryParameters queryParameters)
    {
        var bookQuery = bookRepository.GetQueryable();
        var bookBorrowingRequestDetailQuery = 
            bookBorrowingRequestDetailRepository
                            .GetQueryable()
                            .Where(bbd =>
                                bbd.BookBorrowingRequest.DateRequested >= queryParameters.FromDate 
                                && bbd.BookBorrowingRequest.DateRequested <= queryParameters.ToDate);

        var data = await bookQuery
            .GroupJoin(
                bookBorrowingRequestDetailQuery,
                b => b.Id,
                bbd => bbd.BookId,
                (b, details) => new { Book = b, Details = details }
            )
            .SelectMany(
                x => x.Details.DefaultIfEmpty(),
                (x, detail) => new
                {
                    Book = x.Book,
                    Detail = detail
                }
            )
            .GroupBy(x => new
            {
                x.Book.Id,
                x.Book.Author,
                x.Book.Title,
                x.Book.Quantity,
                x.Book.Available,
                Category = x.Book.Category.Name,
            })
            .Select(g => new BookBorrowingReportResponse
            {
                Id = g.Key.Id,
                Author = g.Key.Author,
                Title = g.Key.Title,
                Category = g.Key.Category,
                Available = g.Key.Available,
                Quantity = g.Key.Quantity,
                TotalBorrow = g.Count(x => x.Detail != null)
            })
            .OrderByDescending(r => r.TotalBorrow)
            .Take(queryParameters.Top)
            .ToListAsync();

        return PaginationResult<BookBorrowingReportResponse>.Create(queryParameters.Top, data);
    }

    public async Task<Result<PaginationResult<CategoryReportResponse>>> GetCategoryReportAsync(CategoryReportQueryParameters queryParameters)
    {
        var bookQuery = bookRepository.GetQueryable();
        var categoryQuery = categoryRepository.GetQueryable();
        var bookBorrowingRequestDetailQuery =
        bookBorrowingRequestDetailRepository
                    .GetQueryable().Where(bbd => bbd.BookBorrowingRequest.DateRequested >= queryParameters.FromDate
                                                && bbd.BookBorrowingRequest.DateRequested <= queryParameters.ToDate);
        var bookRequestedQuery = bookQuery.GroupJoin(
                bookBorrowingRequestDetailQuery,
                b => b.Id,
                bbd => bbd.BookId,
                (b, details) => new { Book = b, Details = details }
            )
            .SelectMany(
                x => x.Details.DefaultIfEmpty(),
                (x, detail) => new
                {
                    Book = x.Book,
                    Detail = detail
                }
            )
            .GroupBy(x => new
            {
                x.Book.Id,
                x.Book.Author,
                x.Book.Title,
                x.Book.Quantity,
                x.Book.CategoryId,
                x.Book.Available
            })
            .Select(g => new BookOfCategoryReportResponse
            {
                BookId = g.Key.Id,
                Name = g.Key.Title,
                Available = g.Key.Available,
                CategoryId = g.Key.CategoryId,
           
                Quantity = g.Key.Quantity,
                TotalBorrow = g.Count(x => x.Detail != null)
            }).OrderByDescending(b => b.TotalBorrow);

        var data = await categoryQuery
            .GroupJoin(
                bookRequestedQuery,
                c => c.Id,
                b => b.CategoryId,
                (c, books) => new
                {
                    Category = c,
                    Books = books
                })
            .Select(x => new CategoryReportResponse
            {
                Id = x.Category.Id,
                Name = x.Category.Name,
                TotalBook = x.Books.Count(),
                QuantityBook = x.Books.Sum(b => (int?)b.Quantity) ?? 0,
                AvailableBook = x.Books.Sum(b => (int?)b.Available) ?? 0,
                TotalBorrowRequest = x.Books.Sum(b => (int?)b.TotalBorrow) ?? 0,
                MostRequestedBook = x.Books.Sum(b => (int?)b.TotalBorrow) != 0 ? x.Books
                    .Select(b => b.Name)
                    .FirstOrDefault() : null,
                MostRequestedBookId = x.Books.Sum(b => (int?)b.TotalBorrow) != 0 ? x.Books
                    .Select(b => (Guid?)b.BookId)
                    .FirstOrDefault() : null,
                RequestCount = x.Books.Sum(b => (int?)b.TotalBorrow) != 0 ?  x.Books
                    .Select(b => (int?)b.TotalBorrow)
                    .FirstOrDefault()  : 0
            })
            .OrderByDescending(c => c.TotalBorrowRequest)
            .Take(queryParameters.Top)
            .ToListAsync();

        return PaginationResult<CategoryReportResponse>.Create(queryParameters.Top, data);
    }

    public async Task<Result<PaginationResult<UserReportResponse>>> GetUserReportAsync(UserEngagementReportQueryParameters queryParameters)
    {
        var userQuery = userRepository
            .GetQueryable()
            .Where(u => u.Role.Name != nameof(RoleName.Admin));

        var bookBorrowingRequestQuery = 
            bookBorrowingRequestRepository
            .GetQueryable()
            .Where(bbd => 
                bbd.DateRequested >= queryParameters.FromDate
                && bbd.DateRequested <= queryParameters.ToDate);
        
        var data = await userQuery
            .GroupJoin(bookBorrowingRequestQuery, 
                        u => u.Id, 
                        bb => bb.RequesterId,
                        (u, bb) => new 
                        {
                            User = u,
                            BookBorrowingRequests = bb,
                            BookReviews = u.BookReviews != null ? u.BookReviews.Count : 0
                        })
            .SelectMany(d => d.BookBorrowingRequests.DefaultIfEmpty(), 
                       (d, bb) => new
                       {
                           User=d.User,
                           BookReviews = d.BookReviews,
                           BookBorrowingRequest = bb
                       })
            .OrderByDescending(b => 
                b.BookBorrowingRequest == null ? 
                DateOnly.MinValue : b.BookBorrowingRequest.DateRequested)
            .GroupBy(d => new
            {
                d.User.Id,
                d.User.Username,
                d.User.Email,
                d.BookReviews,
            })
            .Select(g => new UserReportResponse
            {
                Id = g.Key.Id,
                Username = g.Key.Username,
                Email = g.Key.Email,
                NumberOfBookReview = g.Key.BookReviews,
                ApprovedBookBorrowingRequest = g.Sum(c => (c.BookBorrowingRequest != null && c.BookBorrowingRequest.Status == (int)BookBorrowingStatus.Approved) ? 1 : 0),
                RejectedBookBorrowingRequest = g.Sum(c => (c.BookBorrowingRequest != null && c.BookBorrowingRequest.Status == (int)BookBorrowingStatus.Rejected) ? 1 : 0),
                TotalBookBorrowingRequest = g.Sum(c => (c.BookBorrowingRequest != null) ? 1 : 0),
                LastBorrowedBook = g.FirstOrDefault()!.BookBorrowingRequest == null ? null : g.FirstOrDefault()!.BookBorrowingRequest!.DateRequested,
                
            }).OrderByDescending(u => u.TotalBookBorrowingRequest).Take(queryParameters.Top).ToListAsync();

        var userIds = data.Select(d => d.Id).ToList();
        var userActivitiesQueryParameters = new UserActivitiesQueryParameters
        {
            FromDate = queryParameters.FromDate,
            ToDate = queryParameters.ToDate,
            UserIds = userIds ?? new(),
        };
        var userActivities = await auditLogger.GetUserActivitiesReportAsync(userActivitiesQueryParameters);
        userActivities.ForEach(u =>
        {
            data.FirstOrDefault(d => d.Id == u.UserId)!.ActiveDay = u.ActiveDays;
        });
        return PaginationResult<UserReportResponse>.Create(queryParameters.Top, data);
    }
}
