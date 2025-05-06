
using MIDASM.Domain.Abstract;
using MIDASM.Domain.Enums;
using System.Threading;

namespace MIDASM.Domain.Entities;

public class BookBorrowingRequest : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid RequesterId { get; set; }
    public Guid? ApproverId { get; set; } = default;
    public DateOnly DateRequested { get; set; }
    public DateOnly? DateApproved { get; set; } = default;
    public int Status { get; set; } = (int)BookBorrowingStatus.Waiting;
    public bool IsDeleted {get;set;}


    public User Requester { get; set; } = default!;
    public User? Approver { get; set; } = default;
    public virtual ICollection<BookBorrowingRequestDetail> BookBorrowingRequestDetails { get; set; } = default!;


    public static BookBorrowingRequest Create(DateOnly dateRequest, List<BookBorrowingRequestDetail> bookBorrowingDetails, Guid requestId)
    {
        var booksBorrowingRequest = new BookBorrowingRequest();
        booksBorrowingRequest.RequesterId = requestId;
        booksBorrowingRequest.DateRequested = dateRequest;
        booksBorrowingRequest.BookBorrowingRequestDetails = bookBorrowingDetails;
        return booksBorrowingRequest;
    }

    public static BookBorrowingRequest Copy(BookBorrowingRequest bookBorrowingRequest)
    {
        return new()
        {
            Id = bookBorrowingRequest.Id,
            RequesterId = bookBorrowingRequest.RequesterId,
            ApproverId = bookBorrowingRequest.ApproverId,
            DateRequested = bookBorrowingRequest.DateRequested,
            DateApproved = bookBorrowingRequest.DateApproved,
            Status = bookBorrowingRequest.Status,
            IsDeleted = bookBorrowingRequest.IsDeleted,
        };
    }

    public static void AdjustingStatus(BookBorrowingRequest bookBorrowingRequest, int status, Guid userId)
    {
        bookBorrowingRequest.Status = status;
        bookBorrowingRequest.ApproverId = userId;
        bookBorrowingRequest.DateApproved = DateOnly.FromDateTime(DateTime.UtcNow.Date);
    }

}
