using MIDASS.Application.Commons.Models.Users;
using MIDASS.Domain.Entities;
using MIDASS.Domain.Enums;

namespace MIDASS.Application.Commons.Models.BookBorrowingRequests;

public class BookBorrowingRequestData
{
    public Guid Id { get; set; }
    public BookBorrowingRequestUserResponse Requester { get; set; } = default!;
    public BookBorrowingRequestUserResponse? Approver { get; set; } = default;
    public int BooksBorrowingNumber { get; set; }
    public DateOnly DateRequested { get; set; }
    public DateOnly? DateApproved { get; set; } = default;
    public int Status { get; set; } = (int)BookBorrowingStatus.Waiting;
}


