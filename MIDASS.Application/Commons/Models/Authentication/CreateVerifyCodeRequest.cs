
using FluentValidation;
using MIDASS.Contract.Messages.Validations;

namespace MIDASS.Application.Commons.Models.Authentication;

public class EmailConfirmRequest
{
    public string Username { get; set; } = default!;
    public string Code { get; set; } = default!;
}

public class EmailConfirmRequestValidator : AbstractValidator<EmailConfirmRequest>
{
    public EmailConfirmRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage(AuthenticationValidationMessages.UsernameShouldBeNotEmpty);
        RuleFor(x => x.Code).NotEmpty().WithMessage(AuthenticationValidationMessages.EmailConfirmCodeShouldNotBeEmpty)
                            .Must(x => x.Length == 6).WithMessage(AuthenticationValidationMessages.EmailConfirmCodeInvalid);
    }
}
