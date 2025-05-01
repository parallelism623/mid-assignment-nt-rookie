
using MIDASS.Application.Commons.Models.Books;

namespace MIDASS.Application.Commons.Models.BookBorrowingRequestDetails;

public class BookBorrowedRequestDetailResponse
{
    public Guid Id { get; set; }
    public Guid BookBorrowingRequestId { get; set; }
    public Guid BookId { get; set; }
    public BookResponse Book { get; set; } = default!;
    public string RequesterName { get; set; } = default!;
    public string? ApproverName { get; set; }
    public DateOnly DueDate { get; set; }
    public string? Noted { get; set; }
    public int ExtendDueDateTimes { get; set; }
    public DateOnly? ExtendDueDate { get; set; }
}
