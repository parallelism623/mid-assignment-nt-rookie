
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MIDASM.Application.Commons.Models;
using MIDASM.Application.Commons.Models.BookBorrowingRequestDetails;
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

namespace MIDASM.Persistence.Services;

public class BookBorrowingRequestDetailServices(
    IBookBorrowingRequestDetailRepository bookBorrowingRequestDetailRepository,
    IUserRepository userRepository,
    IBackgroundTaskQueue<Func<IServiceProvider, CancellationToken, ValueTask>> mailSenderBackgroundService
    )
            : IBookBorrowingRequestDetailServices
{
    public async Task<Result<string>> AdjustExtendDueDateAsync(Guid id, int status)
    {
        var bookBorrowedDetail = await bookBorrowingRequestDetailRepository.GetByIdAsync(id, "BookBorrowingRequest", "Book");
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

        await HandleSendMailExtendDueDateStatusChange(bookBorrowedDetail, status);
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

    private async Task HandleSendMailExtendDueDateStatusChange(BookBorrowingRequestDetail bd, int status)
    {
        var user = await userRepository.GetByIdAsync(bd.BookBorrowingRequest.RequesterId);
        if (user != null)
        {
            var mailContent = await FileHelper.GetMailTemplateFile(MailTemplateHelper.MailTemplateAdjustExtendBookDueDate);
            var toEmail = user.Email;
            var statusString = status == 1 ? "Approved" : "Rejected";
            var subject = $"Book extend due date request #{bd.Id} was {statusString}";

            var body = mailContent?.Replace("{{Name}}", user.FirstName + " " + user.LastName)?
                            .Replace("{{Status}}", statusString)?
                            .Replace("{{BookTitle}", bd.Book.Title)
                            .Replace("{{BookId}}", bd.Book.Id.ToString())
                            .Replace("{{OriginalDueDate}}", bd.DueDate.ToString())
                            .Replace("{{NewDueDate}}", bd.ExtendDueDate.ToString());


            await mailSenderBackgroundService.QueueBackgroundWorkItemAsync(async (serviceProvider, c) =>
            {
                var mailServices = serviceProvider.GetRequiredService<IMailServices>();
                await mailServices.SendMailAsync(toEmail, subject, body ?? "", cancellationToken: c);
            });
        }


    }
}
