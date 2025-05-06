using MIDASM.Application.Commons.Models.Users;
using MIDASM.Domain.Entities;
using MIDASM.Domain.Enums;

namespace MIDASM.Application.Commons.Models.BookBorrowingRequests;

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


