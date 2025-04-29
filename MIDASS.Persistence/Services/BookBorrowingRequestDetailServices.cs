
using Microsoft.EntityFrameworkCore;
using MIDASS.Application.Commons.Models;
using MIDASS.Application.Commons.Models.BookBorrowingRequestDetails;
using MIDASS.Application.UseCases;
using MIDASS.Contract.Errors;
using MIDASS.Contract.Messages.Commands;
using MIDASS.Contract.SharedKernel;
using MIDASS.Domain.Enums;
using MIDASS.Domain.Repositories;

namespace MIDASS.Persistence.Services;

public class BookBorrowingRequestDetailServices(IBookBorrowingRequestDetailRepository bookBorrowingRequestDetailRepository)
            : IBookBorrowingRequestDetailServices
{
    public async Task<Result<string>> AdjustExtenDueDateAsync(Guid id, int status)
    {
        var bookBorrowedDetail = await bookBorrowingRequestDetailRepository.GetByIdAsync(id, "BookBorrowingRequest");
        if(bookBorrowedDetail == null)
        {
            return Result<string>.Failure(400, BookBorrowingRequestDetailErrors.BookBorrowedDetailNotFound);
        }
        if(bookBorrowedDetail.BookBorrowingRequest.Status == (int)BookBorrowingStatus.Rejected)
        {
            return Result<string>.Failure(400, BookBorrowingRequestDetailErrors.BookBorrowReject);
        }
        if(bookBorrowedDetail.ExtendDueDate == null)
        {
            return Result<string>.Failure(400, BookBorrowingRequestDetailErrors.BookBorrowedExtendDueDateInvalid);
        }
        if(status == 1)
        {
            bookBorrowedDetail.DueDate = (DateOnly)bookBorrowedDetail.ExtendDueDate;
        }
   
        bookBorrowedDetail.ExtendDueDate = null;
        
        bookBorrowingRequestDetailRepository.Update(bookBorrowedDetail);
        await bookBorrowingRequestDetailRepository.SaveChangesAsync();

        return BookBorrowingRequestDetailCommandMessages.AdjustExtendDueDateRequestSuccess;
    }

    public async Task<Result<PaginationResult<BookBorrowedRequestDetailResponse>>> GetsAsync(QueryParameters queryParameters)
    {

        var pageSize = queryParameters.PageSize;
        var pageIndex = queryParameters.PageIndex;
        var query = bookBorrowingRequestDetailRepository
            .GetQueryable()
            .Where(b => b.BookBorrowingRequest.Status == (int)BookBorrowingStatus.Approved);

        int totalCount = await query.CountAsync();

        var data = await query
                .OrderByDescending(b => b.CreatedAt)
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
                        Id = bd.BookId,

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
}
