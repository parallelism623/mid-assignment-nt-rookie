
using FluentValidation;
using MIDASM.Contract.Messages.Validations;

namespace MIDASM.Application.Commons.Models.Users;

public class DueDatedExtendRequest
{
    public Guid BookBorrowedDetailId { get; set; }
    public DateOnly ExtendDueDate { get; set; }
}


public class DueDatedExtendRequestValidator : AbstractValidator<DueDatedExtendRequest>
{
    public DueDatedExtendRequestValidator()
    {
        RuleFor(x => x.BookBorrowedDetailId).NotEmpty().WithMessage(UserValidationMessages.BookBorrowedExtendDueDateIdMustNotEmpty);
        RuleFor(x => x.ExtendDueDate).NotEmpty().WithMessage(UserValidationMessages.BookBorrowedExtendDueDateExtendDateMustNotEmpty); ;
    }
}