
using FluentValidation;
using MIDASS.Contract.Messages.Validations;
using MIDASS.Domain.Constrants;
using MIDASS.Domain.Entities;

namespace MIDASS.Application.Commons.Models.Users;

public class BookBorrowingRequestCreate
{
    public Guid RequesterId { get; set; }
    public DateOnly DateRequested { get; set; }
    public List<BookBorrowingRequestDetailCreate> BorrowingRequestDetails { get; set; } = default!;
}


public class BookBorrowingRequestDetailCreate
{
    public Guid BookId { get; set; }
    public DateOnly DueDate { get; set; }
    public string? Noted { get; set; } = default;
}

public class BookBorrowingRequestDetailCreateDetailValidator : AbstractValidator<BookBorrowingRequestDetailCreate>
{
    public BookBorrowingRequestDetailCreateDetailValidator()
    {
        RuleFor(bd => bd.BookId)
            .NotEmpty().WithMessage(UserValidationMessages.BookBorrowingDetailBookIdEmpty);
        RuleFor(bd => bd.DueDate)
            .NotEmpty().WithMessage(UserValidationMessages.BookBorrowingDetailDueDateEmpty)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage(UserValidationMessages.BookBorrowingDetailDueDateInvalid);
        RuleFor(bd => bd.Noted)
            .MaximumLength(BookBorrowingRequestDetailValidationRules.MaxLengthNoted)
            .WithMessage(string.Format(UserValidationMessages.BookBorrowingDetailNotedInvalidLength, 
                                       BookBorrowingRequestDetailValidationRules.MaxLengthNoted));
    }
}

public class BookBorrowingRequestCreateValidator : AbstractValidator<BookBorrowingRequestCreate>
{
    public BookBorrowingRequestCreateValidator()
    {
        RuleFor(b => b.RequesterId)
            .NotEmpty().WithMessage(UserValidationMessages.BookBorrowingRequesterIdEmpty);
        RuleFor(b => b.DateRequested)
            .NotEmpty().WithMessage(UserValidationMessages.BookBorrowingDateRequestEmpty);
        RuleFor(b => b.BorrowingRequestDetails.Count)
            .Must(l => l > 0 && l <= 5).WithMessage(UserValidationMessages.BooksBorrowingInSingleRequestShouldInRange);

        RuleFor(b => b.BorrowingRequestDetails.Select(p => p.BookId))
            .Must(l => l.GroupBy(id => id).Count() == l.Count()).WithMessage(UserValidationMessages.BooksBorrowingShouldHaveDistinctBookId);

        RuleForEach(b => b.BorrowingRequestDetails)
            .SetValidator(new BookBorrowingRequestDetailCreateDetailValidator());
    }
}