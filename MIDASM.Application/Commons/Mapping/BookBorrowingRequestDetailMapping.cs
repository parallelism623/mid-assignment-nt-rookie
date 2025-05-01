
using MIDASM.Application.Commons.Models.BookBorrowingRequestDetails;
using MIDASM.Domain.Entities;
using System.Net;

namespace MIDASM.Application.Commons.Mapping;

public static class BookBorrowingRequestDetailMapping
{
    public static BookBorrowedRequestDetailResponse ToBookBorrowedRequestDetailResponse(this BookBorrowingRequestDetail bookBorrowingRequestDetail)
    {
        return new()
        {
            Id = bookBorrowingRequestDetail.Id,
            DueDate = bookBorrowingRequestDetail.DueDate,
            BookBorrowingRequestId = bookBorrowingRequestDetail.BookBorrowingRequestId,
            BookId = bookBorrowingRequestDetail.BookId,
            Book = bookBorrowingRequestDetail.Book.ToBookResponse(),
            Noted = bookBorrowingRequestDetail.Noted,
            ExtendDueDateTimes = bookBorrowingRequestDetail.ExtendDueDateTimes
        };
    }
}
