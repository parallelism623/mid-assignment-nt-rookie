
using Mapster;
using MIDASS.Application.Commons.Models.BookBorrowingRequests;
using MIDASS.Application.Commons.Models.Users;
using MIDASS.Domain.Entities;

namespace MIDASS.Application.Commons.Mapping;

public static class BookBorrowingRequestMapping
{
    public static BookBorrowingRequestResponse ToBookBorrowingRequestResponse(this BookBorrowingRequest bookBorrowing)
    {
        var response = bookBorrowing.Adapt<BookBorrowingRequestResponse>();
        response.Approver = bookBorrowing.Approver.Adapt<BookBorrowingRequestUserResponse>();
        return response;
    }

    public static BookBorrowingRequest ToBookBorrowingRequest(this BookBorrowingRequestCreate bookBorrowingCreate)
    {
        return BookBorrowingRequest.Create(bookBorrowingCreate.DateRequested, 
            bookBorrowingCreate.BorrowingRequestDetails.ToBookBorrowingRequestDetails(),
            bookBorrowingCreate.RequesterId);
    }

    public static BookBorrowingRequestDetail ToBookBorrowingRequestDetail(
        this BookBorrowingRequestDetailCreate bookBorrowingRequestDetail)
    {
        return BookBorrowingRequestDetail.Create(bookBorrowingRequestDetail.BookId,
            bookBorrowingRequestDetail.DueDate, bookBorrowingRequestDetail.Noted);
    }
    public static List<BookBorrowingRequestDetail> ToBookBorrowingRequestDetails(
        this List<BookBorrowingRequestDetailCreate> bookBorrowingRequestDetail)
    {
        return bookBorrowingRequestDetail.Select(p => p.ToBookBorrowingRequestDetail())
            .ToList();
    }

    public static BookBorrowingRequestData ToBookBorrowingRequestData(this BookBorrowingRequest bookBorrowingRequest)
    {
        return new()
        {
            Id = bookBorrowingRequest.Id,
            DateApproved = bookBorrowingRequest.DateApproved,
            Status = bookBorrowingRequest.Status,
            DateRequested = bookBorrowingRequest.DateRequested,
            Approver =
                bookBorrowingRequest.Approver == null
                    ? default
                    : new()
                    {
                        Id = bookBorrowingRequest.Approver.Id,
                        FirstName = bookBorrowingRequest.Approver.FirstName,
                        LastName = bookBorrowingRequest.Approver.LastName,
                        FullName = bookBorrowingRequest.Approver.FirstName + " " + bookBorrowingRequest.Approver.LastName
                    },
            Requester = bookBorrowingRequest.Requester == default!
                ? default!
                : new()
                {
                    Id = bookBorrowingRequest.Requester.Id,
                    FirstName = bookBorrowingRequest.Requester.FirstName,
                    LastName = bookBorrowingRequest.Requester.LastName,
                    FullName = bookBorrowingRequest.Requester.FirstName + " " + bookBorrowingRequest.Requester.LastName
                },
            BooksBorrowingNumber = bookBorrowingRequest.BookBorrowingRequestDetails.Count
        };
    }
}
