
namespace MIDASM.Contract.Messages.Validations;

public static class CategoryValidationMessages
{
    public const string CategoryNameShouldNotBeEmpty = "Category name should not be empty";
    public const string CategoryIdShouldNotBeEmpty = "Category id should not be empty";
    public const string CategoryNameShouldLessThanOrEqualMaxLength = "Category name should less than or equal {0}";
    public const string CategoryDescriptionShouldLessThanOrEqualMaxLength = "Category description should less than or equal {0}";
}
