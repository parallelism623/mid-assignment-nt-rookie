
namespace MIDASS.Contract.Messages.Validations;

public static class BookReviewValidationMessages
{
    public const string BookIdOfReviewMustBeNotEmpty = "Book id of review must be not null";
    public const string ReviewerIdOfReviewMustBeNotEmpty = "Reviewer id of review must be not null";
    public const string ReviewTitleMustBeLessThanOrEqualMaxLength = "Review title must be less than or equal {0}";
    public const string ReviewContentMustBeLessThanOrEqualMaxLength = "Review content must be less than or equal {0}";
    public const string ReviewTitleMustBeNotEmpty = "Review title must be not empty";
    public const string ReviewContentMustBeNotEmpty = "Review content must be not empty";
    public const string ReviewRatingMustBeInRange = "Review rating must be in range 1 - 5";
    public const string DateReviewMustBeNotEmpty = "Date review must be not empty";
}
