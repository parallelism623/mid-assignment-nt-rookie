
using FluentValidation;
using MIDASM.Contract.Messages.Validations;
using MIDASM.Domain.Constrants;

namespace MIDASM.Application.Commons.Models.BookReviews;

public class CreateBookReviewRequest
{
    public Guid BookId { get; set; }
    public Guid ReviewerId { get; set; }
    public DateOnly DateReview { get; set; }
    public int Rating { get; set; }
    public string? Content { get; set; } = default!;
    public string Title { get; set; } = default!;
}


public class CreateBookReviewRequestValidator : AbstractValidator<CreateBookReviewRequest>
{
    public CreateBookReviewRequestValidator()
    {
        RuleFor(r => r.BookId).NotEmpty()
            .WithMessage(BookReviewValidationMessages.BookIdOfReviewMustBeNotEmpty);
        RuleFor(r => r.ReviewerId).NotEmpty()
            .WithMessage(BookReviewValidationMessages.ReviewerIdOfReviewMustBeNotEmpty);
        RuleFor(r => r.DateReview).NotEmpty()
            .WithMessage(BookReviewValidationMessages.DateReviewMustBeNotEmpty);
        RuleFor(r => r.Rating).Must(r => r >= 1 && r <= 5)
            .WithMessage(BookReviewValidationMessages.ReviewRatingMustBeInRange);
        RuleFor(r => r.Title)
            .NotEmpty()
            .WithMessage(BookReviewValidationMessages.ReviewTitleMustBeNotEmpty)
            .Must(t => t.Length <= BookReviewValidationRules.TitleMaxLength)
            .WithMessage(string.Format(BookReviewValidationMessages.ReviewTitleMustBeLessThanOrEqualMaxLength, 
                                       BookReviewValidationRules.TitleMaxLength));
        RuleFor(r => r.Content)
            .Must(r => r!.Length <= BookReviewValidationRules.ContentMaxLength)
            .WithMessage(string.Format(BookReviewValidationMessages.ReviewContentMustBeLessThanOrEqualMaxLength, 
                                       BookReviewValidationRules.ContentMaxLength))
            .When(r => !string.IsNullOrEmpty(r.Content));
    }
}