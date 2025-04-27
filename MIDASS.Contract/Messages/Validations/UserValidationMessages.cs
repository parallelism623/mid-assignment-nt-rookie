namespace MIDASS.Contract.Messages.Validations;

public static class UserValidationMessages
{
    public const string BookBorrowingDateRequestEmpty = "Book borrowing request date should not be empty";
    public const string BookBorrowingRequesterIdEmpty = "Book borrowing requester id should not be empty";
    public const string BookBorrowingDetailBookIdEmpty = "Book borrowing detail has empty book id";
    public const string BookBorrowingDetailDueDateEmpty = "Book borrowing detail has empty due date";
    public const string BookBorrowingDetailDueDateInvalid = "Book borrowing detail due date invalid";

    public const string BookBorrowingDetailNotedInvalidLength =
        "Book borrowing detail noted should not be greater than 2000";

    public const string BooksBorrowingInSingleRequestShouldInRange = "Books borrowing should be in range 1 - 5";

    public const string BooksBorrowingShouldHaveDistinctBookId =
        "Books id in borrowing request should be distinct with other";
}
