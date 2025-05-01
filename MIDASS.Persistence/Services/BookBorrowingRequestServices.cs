using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MIDASS.Application.Commons.Mapping;
using MIDASS.Application.Commons.Models.BookBorrowingRequests;
using MIDASS.Application.Services.Authentication;
using MIDASS.Application.Services.HostedServices.Abstract;
using MIDASS.Application.Services.Mail;
using MIDASS.Application.UseCases;
using MIDASS.Contract.Errors;
using MIDASS.Contract.Messages.Commands;
using MIDASS.Contract.SharedKernel;
using MIDASS.Domain.Entities;
using MIDASS.Domain.Enums;
using MIDASS.Domain.Repositories;
using MIDASS.Persistence.Specifications;

namespace MIDASS.Persistence.Services;

public class BookBorrowingRequestServices(
    IBookBorrowingRequestRepository bookBorrowingRequestRepository, 
    IExecutionContext executionContext,
    IBookRepository bookRepository,
    IUserRepository userRepository,
    IBackgroundTaskQueue<Func<IServiceProvider, CancellationToken,ValueTask>> mailSenderBackgroundService,
    IWebHostEnvironment env)
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
            var templatePath = Path.Combine(env.ContentRootPath, "EmailTemplates", "RequestStatusChanged.html");
            string content = await System.IO.File.ReadAllTextAsync(templatePath) ?? "";

            var body = content?.Replace("@Model.Name", user.FirstName + user.LastName)?
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