
using MIDASM.Contract.SharedKernel;

namespace MIDASM.Contract.Errors;
public static class UserErrorMessages
{
    public const string UserNotFound = "User not found";
    public const string UserReachBorrowingRequestLimit = "User has been reached borrowing request limit in this month";

    public const string UserBorrowingRequestBooksInvalid = "Some books in borrowing request invalid";

    public const string UserCannotBeInCurrentSession = "User can not be in current session";

    public const string SomeBooksInBooksBorrowingRequestUnavailable =
        "Some books in books borrowing request unavailable";

    public const string ErrorOccurWhenCreateBookBorrowingRequest = "Error occurr when create book borrowing, please try again";
    public const string ErrorOccurWhenUpdateBookBorrowingRequest = "Error occurr when update book borrowing, please try again";

    public const string BookBorrowedNotExistsCanNotExtendDueDate = "Book borrowed not exists can not extend due date";
    public const string BookBorrowRejectCanNotExtendDueDate = "Book borrowing request status is rejected, can not extend due date";
    public const string BookBorrowedExtendDueDateTimesReachLimit = "Book borrowed extend due date times reach limit";
    public const string BookBorrowedNewExtendDueDateInValid = "Book borrowed new extend due date in valid";

    public const string UserRoleNotFound = "User role not found";
    public const string UserNameAlreadyExists = "Username already exists";
    public const string EmailAlreadyExists = "Email already exists";

    public const string PasswordNotCorrect = "Current password incorrect";
}
public static class UserErrors
{
    public static Error UserNotFound { get; set; } = new("UserNotFound", UserErrorMessages.UserNotFound);

    public static Error UserReachBorrowingRequestLimit { get; set; } =
        new("UserReachBorrowingRequestLimit", UserErrorMessages.UserReachBorrowingRequestLimit);

    public static Error UserBorrowingRequestBooksInvalid { get; set; } = new("UserBorrowingRequestBooksInvalid",
        UserErrorMessages.UserBorrowingRequestBooksInvalid);

    public static Error UserCannotBeInCurrentSession { get; set; } =
        new("UserCannotBeInCurrentSession", UserErrorMessages.UserCannotBeInCurrentSession);

    public static Error SomeBooksInBooksBorrowingRequestUnavailable { get; set; } =
        new("SomeBooksInBooksBorrowingRequestUnavailable",
            UserErrorMessages.SomeBooksInBooksBorrowingRequestUnavailable);

    public static Error ErrorOccurWhenCreateBookBorrowingRequest { get; set; } = new("ErrorOccurWhenCreateBookBorrowingRequest",
        UserErrorMessages.ErrorOccurWhenCreateBookBorrowingRequest);


    public static Error ErrorOccurWhenUpdateBookBorrowingRequest { get; set; } = new("ErrorOccurWhenUpdateBookBorrowingRequest",
        UserErrorMessages.ErrorOccurWhenUpdateBookBorrowingRequest);

    public static Error BookBorrowedNotExistsCanNotExtendDueDate => new(nameof(BookBorrowedNotExistsCanNotExtendDueDate), UserErrorMessages.BookBorrowedNotExistsCanNotExtendDueDate);
    public static Error BookBorrowRejectCanNotExtendDueDate => new(nameof(BookBorrowRejectCanNotExtendDueDate), UserErrorMessages.BookBorrowRejectCanNotExtendDueDate);
    public static Error BookBorrowedExtendDueDateTimesReachLimit => new(nameof(BookBorrowedExtendDueDateTimesReachLimit), UserErrorMessages.BookBorrowedNewExtendDueDateInValid);
    public static Error BookBorrowedNewExtendDueDateInValid => new(nameof(BookBorrowedNewExtendDueDateInValid), UserErrorMessages.BookBorrowedNewExtendDueDateInValid);
    public static Error UserRoleNotFound => new(nameof(UserRoleNotFound), UserErrorMessages.UserRoleNotFound);

    public static Error UsernameAlreadyExists => new(nameof(UsernameAlreadyExists), UserErrorMessages.EmailAlreadyExists);
    public static Error EmailAlreadyExists => new(nameof(EmailAlreadyExists), UserErrorMessages.EmailAlreadyExists);

    public static Error PasswordNotCorrect => new(nameof(PasswordNotCorrect), UserErrorMessages.PasswordNotCorrect);
}
