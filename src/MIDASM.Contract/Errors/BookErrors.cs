
using MIDASM.Contract.SharedKernel;

namespace MIDASM.Contract.Errors;

public static class BookErrorMessages
{
    public const string BookCanNotCreateDueToInvalidCategory = "Book can not be created due to invalid category";
    public const string BookCanNotFound = "Book can not found";
    public const string BookCanNotUpdateDueToInvalidCategory = "Book can not be updated due to invalid category";
    public const string BookCanNotDeletedDueToExistsBorrowRequest = "Book can not be deleted due to exists borrow request";
    public const string BookQuantityAddedInvalid = "Book quantity added is not valid";
    public const string BookIdInvalid = "Book id invalid";
}
public static class BookErrors
{
    public static Error BookCanNotCreateDueToInvalidCategory =>
        new("BookCanNotCreateDueToInvalidCategory", BookErrorMessages.BookCanNotCreateDueToInvalidCategory);

    public static Error BookCanNotFound => new("BookCanNotFound", BookErrorMessages.BookCanNotFound);

    public static Error BookCanNotUpdateDueToInvalidCategory =>
        new("BookCanNotUpdateDueToInvalidCategory", BookErrorMessages.BookCanNotUpdateDueToInvalidCategory);

    public static Error BookCanNotDeletedDueToExistsBorrowRequest =>
        new("BookCanNotUpdateDueToInvalidCategory", BookErrorMessages.BookCanNotDeletedDueToExistsBorrowRequest);

    public static Error BookQuantityAddedInvalid =>
        new("BookQuantityAddedInvalid", BookErrorMessages.BookQuantityAddedInvalid);

    public static Error BookIdInvalid => new("BookIdInvalid", BookErrorMessages.BookIdInvalid);
}
