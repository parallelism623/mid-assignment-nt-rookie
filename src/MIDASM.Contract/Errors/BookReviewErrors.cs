
using MIDASM.Contract.SharedKernel;

namespace MIDASM.Contract.Errors;

public static class BookReviewErrorMessages
{
    public static string ReviewerNotExists => "Reviewer of book review not exists";
    public static string BookNotExists => "Book of book review not exists";

    public static string UserHasNotBorrowedBook => "User have not borrowed this book";
}

public static class BookReviewErrors
{
    public static Error ReviewerNotExists => new(nameof(ReviewerNotExists), BookReviewErrorMessages.ReviewerNotExists);
    public static Error BookNotExists => new(nameof(BookNotExists), BookReviewErrorMessages.BookNotExists);
    public static Error UserHasNotBorrowedBook => new(nameof(UserHasNotBorrowedBook), BookReviewErrorMessages.UserHasNotBorrowedBook);
}
