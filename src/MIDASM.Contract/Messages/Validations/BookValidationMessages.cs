
namespace MIDASM.Contract.Messages.Validations;

public static class BookValidationMessages
{
    public const string IdShouldNotBeEmpty = "Book id should not be empty";
    public const string TitleShouldNotBeEmpty = "Book title should not be empty";
    public const string AuthorShouldNotBeEmpty = "Book author should not be empty";
    public const string TitleShouldLessEqualThanMaxLength = "Book title should less equal than {0}";
    public const string DescriptionShouldLessEqualThanMaxLength = "Book description should less equal than {0}";
    public const string AuthorShouldLessEqualThanMaxLength = "Book author should less equal than {0}";

    public const string BookAvailableShouldLessThanOrEqualQuantity =
        "Book available should be less than or equal quantity";

    public const string BookQuantityShouldGreaterThanZero = "Book quantity should greater than zero";
    public const string BookAvailableShouldGreaterThanZero = "Book available should greater than zero";
    public const string NewBookAvailableShouldGreaterThanZero = "New book available should be greater than zero";
    public const string BookMustHaveCategory = "Book must have category";
}
