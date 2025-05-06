using FluentValidation;
using MIDASM.Contract.Messages.Validations;
using MIDASM.Domain.Enums;

namespace MIDASM.Application.Commons.Models.BookBorrowingRequests;

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
