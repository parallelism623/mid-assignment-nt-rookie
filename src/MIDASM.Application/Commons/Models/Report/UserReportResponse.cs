
namespace MIDASM.Application.Commons.Models.Report;

public class UserReportResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string Username { get; set; } = default!;
    public int TotalBookBorrowingRequest { get; set; }

    public int ApprovedBookBorrowingRequest { get; set; } 
    public int RejectedBookBorrowingRequest { get; set; }
    public int TotalBookBorrowing { get; set; }
    public int? ActiveDay { get; set; } = default!;
    public DateOnly? LastBorrowedBook { get; set; }
    public int? NumberOfBookReview { get; set; } = default!;
}
