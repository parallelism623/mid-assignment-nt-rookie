﻿namespace MIDASM.Contract.Messages.Validations;

public static class UserValidationMessages
{
    public const string UserIdMustBeNotEmpty = "User ID must be not empty";

    public const string BookBorrowingDateRequestEmpty = "Book borrowing request date should not be empty";
    public const string BookBorrowingRequesterIdEmpty = "Book borrowing requester id should not be empty";
    public const string BookBorrowingDetailBookIdEmpty = "Book borrowing detail has empty book id";
    public const string BookBorrowingDetailDueDateEmpty = "Book borrowing detail has empty due date";
    public const string BookBorrowingDetailDueDateInvalid = "Book borrowing detail due date invalid";


    public const string BookBorrowingDetailNotedInvalidLength =
        "Book borrowing detail noted should not be greater than {0}";

    public const string BooksBorrowingInSingleRequestShouldInRange = "Books borrowing should be in range 1 - 5";
    public const string BooksBorrowingRequestLimitShouldInRange = "Books borrowing requset limit should be in range 0 - 3";
    public const string BooksBorrowingShouldHaveDistinctBookId =
        "Books id in borrowing request should be distinct with other";

    public const string BookBorrowedExtendDueDateIdMustNotEmpty = "Book borrowed extend due date id must not empty";
    public const string BookBorrowedExtendDueDateExtendDateMustNotEmpty = "Book borrowed extend due date extend date must not empty";
    public const string UserRoleMustNotEmpty = "User role must not empty";
}
