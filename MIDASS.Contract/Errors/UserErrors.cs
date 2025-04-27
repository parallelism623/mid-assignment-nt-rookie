
using MIDASS.Contract.SharedKernel;

namespace MIDASS.Contract.Errors;
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
}
