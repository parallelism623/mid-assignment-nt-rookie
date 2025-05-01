
using MIDASM.Domain.Abstract;
using MIDASM.Domain.Enums;

namespace MIDASM.Domain.Entities;

public class BookBorrowingRequest : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid RequesterId { get; set; }
    public Guid? ApproverId { get; set; } = default;
    public User Requester { get; set; } = default!;
    public User? Approver { get; set; } = default;
    public DateOnly DateRequested { get; set; }
    public DateOnly? DateApproved { get; set; } = default;
    public int Status { get; set; } = (int)BookBorrowingStatus.Waiting;
    public bool IsDeleted {get;set;}
    public virtual ICollection<BookBorrowingRequestDetail> BookBorrowingRequestDetails { get; set; } = default!;


    public static BookBorrowingRequest Create(DateOnly dateRequest, List<BookBorrowingRequestDetail> bookBorrowingDetails, Guid requestId)
    {
        var booksBorrowingRequest = new BookBorrowingRequest();
        booksBorrowingRequest.RequesterId = requestId;
        booksBorrowingRequest.DateRequested = dateRequest;
        booksBorrowingRequest.BookBorrowingRequestDetails = bookBorrowingDetails;
        return booksBorrowingRequest;
    }
}
