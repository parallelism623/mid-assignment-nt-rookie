namespace MIDASS.Application.Commons.Models.Users;

public class BookBorrowingRequestResponse
{
    public Guid Id { get; set; }
    public BookBorrowingRequestUserResponse? Approver { get; set; } = default;
    public DateOnly DateRequested { get; set; }
    public DateOnly? DateApproved { get; set; }
    public int Status { get; set; } = default!;
    public int BooksBorrowingNumber { get; set; }
}

public class BookBorrowingRequestUserResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FullName { get; set; } = default!;
}
