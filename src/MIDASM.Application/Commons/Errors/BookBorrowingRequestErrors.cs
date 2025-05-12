using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.Commons.Errors;

public static class BookBorrowingRequestErrorMessage
{
    public const string NotFound = "Book borrowing request not found";
    public const string CanNotUpdateCurrentStatus = "Book borrowing request can not update current status";
}
public static class BookBorrowingRequestErrors
{
    public static Error NotFound => new("Not found", BookBorrowingRequestErrorMessage.NotFound);

    public static Error CanNotUpdateCurrentStatus => new("Status can not change",
        BookBorrowingRequestErrorMessage.CanNotUpdateCurrentStatus);
}
