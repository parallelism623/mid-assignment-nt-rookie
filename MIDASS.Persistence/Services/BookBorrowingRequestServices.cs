    
using Microsoft.EntityFrameworkCore;
using MIDASS.Application.Commons.Mapping;
using MIDASS.Application.Commons.Models.BookBorrowingRequests;
using MIDASS.Application.Services.Authentication;
using MIDASS.Application.UseCases;
using MIDASS.Contract.Errors;
using MIDASS.Contract.Messages.Commands;
using MIDASS.Contract.SharedKernel;
using MIDASS.Domain.Entities;
using MIDASS.Domain.Enums;
using MIDASS.Domain.Repositories;
using MIDASS.Persistence.Repositories;
using MIDASS.Persistence.Specifications;

namespace MIDASS.Persistence.Services;

public class BookBorrowingRequestServices(IBookBorrowingRequestRepository bookBorrowingRequestRepository, 
    IExecutionContext executionContext,
    IBookRepository bookRepository)
    : IBookBorrowingRequestServices
{
    public async Task<Result<PaginationResult<BookBorrowingRequestData>>> GetsAsync(BookBorrowingRequestQueryParameters queryParameters)
    {
        int pageIndex = queryParameters.PageIndex;
        int pageSize = queryParameters.PageSize;
        IQueryable<BookBorrowingRequest> query = bookBorrowingRequestRepository.GetQueryable();

        BookBorrowingRequestByQueryParametersSpecification querySpecification = new(queryParameters);

        query = querySpecification.GetQuery(query);

        int totalCount = await query.CountAsync();

        List<BookBorrowingRequestData> data = await query.Skip(pageSize * (pageIndex - 1)).Take(pageSize)
            .Select(p => p.ToBookBorrowingRequestData())
            .ToListAsync();


        return PaginationResult<BookBorrowingRequestData>.Create(pageSize, pageIndex, totalCount, data);
    }

    public async Task<Result<string>> ChangeStatusAsync(BookBorrowingStatusUpdateRequest statusUpdateRequest)
    {
        var bookBorrowingRequest = await bookBorrowingRequestRepository.GetByIdAsync(statusUpdateRequest.Id, "BorrowingRequestDetails");

        if (bookBorrowingRequest == null)
        {
            return Result<string>.Failure(400, BookBorrowingRequestErrors.NotFound);
        }

        if (bookBorrowingRequest.Status != (int)BookBorrowingStatus.Waiting)
        {
            return Result<string>.Failure(400, BookBorrowingRequestErrors.CanNotUpdateCurrentStatus);
        }
        var booksIdRequest = bookBorrowingRequest.BookBorrowingRequestDetails.Select(bd => bd.BookId).ToList();
        var books = await bookRepository.GetByIdsAsync(booksIdRequest);

        bookBorrowingRequest.Status = statusUpdateRequest.Status;
        bookBorrowingRequest.ApproverId = executionContext.GetUserId();

        bookBorrowingRequestRepository.Update(bookBorrowingRequest);
        await bookBorrowingRequestRepository.SaveChangesAsync();

        return BookBorrowingRequestCommandMessages.ChangeStatusSuccess;
    }
}