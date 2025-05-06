
using FluentValidation;

namespace MIDASM.Application.Commons.Models.BookBorrowingRequestDetails;

public class BookBorrowedExtendDueDateRequest
{
    public int Status { get; set; }
}

public class BookBorrowedExtendDueDateRequestValidator : AbstractValidator<BookBorrowedExtendDueDateRequest>
{
    public BookBorrowedExtendDueDateRequestValidator()
    {
        RuleFor(x => x.Status).Must(x => x == 1 || x == 0)
            .WithMessage("Adjusting extend due date request should be Approve or Reject");
    }
}
