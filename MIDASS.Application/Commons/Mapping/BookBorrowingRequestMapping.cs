
using Mapster;
using Microsoft.VisualBasic;
using MIDASS.Application.Commons.Models.BookBorrowingRequests;
using MIDASS.Application.Commons.Models.Users;
using MIDASS.Domain.Entities;

namespace MIDASS.Application.Commons.Mapping;

public static class BookBorrowingRequestMapping
{
    public static BookBorrowingRequestResponse ToBookBorrowingRequestResponse(this BookBorrowingRequest bookBorrowing)
    {
        var response = bookBorrowing.Adapt<BookBorrowingRequestResponse>();
        response.Approver = bookBorrowing.Approver.Adapt<BookBorrowingRequestUserResponse>() ;
        if(response.Approver != null)
        {
            response.Approver.FullName = response.Approver.FirstName + response.Approver.LastName;
        }    
        response.BooksBorrowingNumber = bookBorrowing.BookBorrowingRequestDetails.Count;
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
    public static BookBorrowingRequestDetailResponse ToBookBorrowingRequestDetailResponse(this BookBorrowingRequest bookBorrowingRequest)
    {
        return new()
        {
            Id = bookBorrowingRequest.Id,
            Status = bookBorrowingRequest.Status,
            Items = bookBorrowingRequest.BookBorrowingRequestDetails
                    .Select(p => p.ToBookBorrowingRequestDetailItemResponse())
                    .ToList()
        };
    }

    public static BookBorrowingRequestDetailItemResponse ToBookBorrowingRequestDetailItemResponse(this BookBorrowingRequestDetail bookBorrowingRequestDetail)
    {
        return new()
        {
            BookId = bookBorrowingRequestDetail.BookId,
            Title = bookBorrowingRequestDetail.Book.Title,
            Author = bookBorrowingRequestDetail.Book.Author,
            Category = new()
            {
                Id = bookBorrowingRequestDetail.Book.Category.Id,
                Name = bookBorrowingRequestDetail.Book.Category.Name
            },
            DueDate = bookBorrowingRequestDetail.DueDate,
            Noted = bookBorrowingRequestDetail.Noted
        };
    }
}
