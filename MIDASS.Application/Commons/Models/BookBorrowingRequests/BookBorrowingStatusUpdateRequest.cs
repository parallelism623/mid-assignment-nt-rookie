using FluentValidation;
using MIDASS.Contract.Messages.Validations;
using MIDASS.Domain.Enums;

namespace MIDASS.Application.Commons.Models.BookBorrowingRequests;

public class BookBorrowingStatusUpdateRequest
{
    public Guid Id { get; set; }
    public int Status { get; set; }
}


public class BookBorrowingStatusUpdateRequestValidator : AbstractValidator<BookBorrowingStatusUpdateRequest>
{
    public BookBorrowingStatusUpdateRequestValidator()
    {
        RuleFor(bb => bb.Id)
            .NotEmpty().WithMessage(BookBorrowingRequestValidationMessages.IdShouldNotBeEmpty);
        RuleFor(bb => bb.Status)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(Enum.GetNames(typeof(BookBorrowingStatus)).Length);
    }
}
