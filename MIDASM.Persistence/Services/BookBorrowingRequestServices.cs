using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MIDASM.Application.Commons.Mapping;
using MIDASM.Application.Commons.Models.BookBorrowingRequests;
using MIDASM.Application.Services.Authentication;
using MIDASM.Application.Services.HostedServices.Abstract;
using MIDASM.Application.Services.Mail;
using MIDASM.Application.UseCases;
using MIDASM.Contract.Errors;
using MIDASM.Contract.Helpers;
using MIDASM.Contract.Messages.Commands;
using MIDASM.Contract.SharedKernel;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Enums;
using MIDASM.Domain.Repositories;
using MIDASM.Persistence.Specifications;

namespace MIDASM.Persistence.Services;

public class BookBorrowingRequestServices(
    IBookBorrowingRequestRepository bookBorrowingRequestRepository, 
    IExecutionContext executionContext,
    IBookRepository bookRepository,
    IUserRepository userRepository,
    IBackgroundTaskQueue<Func<IServiceProvider, CancellationToken,ValueTask>> mailSenderBackgroundService)
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

        List<BookBorrowingRequestData> data = await query.OrderByDescending(b => b.DateRequested).Skip(pageSize * (pageIndex - 1)).Take(pageSize)
            .Select(p => p.ToBookBorrowingRequestData())
            .ToListAsync();


        return PaginationResult<BookBorrowingRequestData>.Create(pageSize, pageIndex, totalCount, data);
    }

    public async Task<Result<string>> ChangeStatusAsync(BookBorrowingStatusUpdateRequest statusUpdateRequest)
    {
        var bookBorrowingRequest = await bookBorrowingRequestRepository.GetByIdAsync(statusUpdateRequest.Id, "BookBorrowingRequestDetails");

        if (bookBorrowingRequest == null)
        {
            return Result<string>.Failure(400, BookBorrowingRequestErrors.NotFound);
        }

        if (bookBorrowingRequest.Status != (int)BookBorrowingStatus.Waiting)
        {
            return Result<string>.Failure(400, BookBorrowingRequestErrors.CanNotUpdateCurrentStatus);
        }

        if(statusUpdateRequest.Status == (int)BookBorrowingStatus.Rejected)
        {
            await HandleRejectBookBorrowingRequest(bookBorrowingRequest);
        }
        bookBorrowingRequest.Status = statusUpdateRequest.Status;
        bookBorrowingRequest.ApproverId = executionContext.GetUserId();
        bookBorrowingRequest.DateApproved = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        bookBorrowingRequestRepository.Update(bookBorrowingRequest);
        await bookBorrowingRequestRepository.SaveChangesAsync();
        await HandleSendMailChangeStatusRequest(bookBorrowingRequest);
        return BookBorrowingRequestCommandMessages.ChangeStatusSuccess;
    }

    public async Task<Result<BookBorrowingRequestDetailResponse>> GetDetailAsync(Guid id)
    {
        var bookBorrowingRequest = await bookBorrowingRequestRepository.GetDetailAsync(id);

        return bookBorrowingRequest?.ToBookBorrowingRequestDetailResponse() ?? default!;
    }

    private async Task HandleRejectBookBorrowingRequest(BookBorrowingRequest bookBorrowingRequest)
    {
        var booksIdRequest = bookBorrowingRequest.BookBorrowingRequestDetails.Select(bd => bd.BookId).ToList();
        var books = await bookRepository.GetByIdsAsync(booksIdRequest);
        books.ForEach(b => b.Available += 1);
        bookRepository.UpdateRange(books);
        await bookRepository.SaveChangesAsync();
    }
    private async ValueTask HandleSendMailChangeStatusRequest(BookBorrowingRequest bookBorrowingRequest)
    {
        var user = await userRepository.GetByIdAsync(bookBorrowingRequest.RequesterId);
        if(user != null)
        {
            var toEmail = user.Email;
            var status = Enum.GetName(typeof(BookBorrowingStatus), bookBorrowingRequest.Status);
            var subject = $"Book borrowing request #{bookBorrowingRequest.Id} was {status}";

            string content = await FileHelper.GetMailTemplateFile(MailTemplateHelper.MailTemplateUpdateBookBorrowingStatus);

            var body = content?.Replace("@Model.Name", user.FirstName + " " + user.LastName)?
                            .Replace("@Model.Status", status)?
                            .Replace("@Model.RequestDate", bookBorrowingRequest.DateRequested.ToString("dd/MM/yyyy"));


                await mailSenderBackgroundService.QueueBackgroundWorkItemAsync(async (serviceProvider, c) =>
            {
                var mailServices = serviceProvider.GetRequiredService<IMailServices>();
                await mailServices.SendMailAsync(toEmail, subject, body ?? "", cancellationToken: c);
            });
        }
    }
}