namespace MIDASS.Application.Commons.Models.Users;

public class BookBorrowingRequestResponse
{
    public Guid Id { get; set; }
    public BookBorrowingRequestUserResponse? Approver { get; set; } = default;
    public DateOnly DateRequested { get; set; }
    public DateOnly? DateApproved { get; set; }
    public string Status { get; set; } = default!;
}

public class BookBorrowingRequestUserResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FullName { get; set; } = default!;
}
