using MIDASM.Contract.SharedKernel;
using MIDASM.Domain.SharedKernel;

namespace MIDASM.Application.Commons.Errors;


public static class BookBorrowingRequestDetailErrorMessages
{
    public const string BookBorrowedDetailNotFound = "Book borrowed detail not found";
    public const string BookBorrowReject = "Book borrowing request is rejected";
    public const string BookBorrowedExtendDueDateInvalid = "Book borrowing extend due date invalid";
}
public static class BookBorrowingRequestDetailErrors
{
    public static Error BookBorrowedDetailNotFound => new(nameof(BookBorrowedDetailNotFound),
                        BookBorrowingRequestDetailErrorMessages.BookBorrowedDetailNotFound);
    public static Error BookBorrowReject => new(nameof(BookBorrowReject), BookBorrowingRequestDetailErrorMessages.BookBorrowReject);
    public static Error BookBorrowedExtendDueDateInvalid => new(nameof(BookBorrowedExtendDueDateInvalid),
        BookBorrowingRequestDetailErrorMessages.BookBorrowedExtendDueDateInvalid);
}
