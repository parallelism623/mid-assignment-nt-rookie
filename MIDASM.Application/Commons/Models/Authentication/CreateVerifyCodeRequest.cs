
using FluentValidation;
using MIDASM.Contract.Messages.Validations;
using MIDASM.Domain.Constrants;

namespace MIDASM.Application.Commons.Models.Authentication;

public class EmailConfirmRequest
{
    public string Username { get; set; } = default!;
    public string Code { get; set; } = default!;
}

public class EmailConfirmRequestValidator : AbstractValidator<EmailConfirmRequest>
{
    public EmailConfirmRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage(AuthenticationValidationMessages.UsernameShouldBeNotEmpty)
            .Matches(UserValidationRules.RegexPatternUsername)
            .WithMessage(string.Format(AuthenticationValidationMessages.UsernameShouldMatchesRegexPattern,
                                       UserValidationRules.MaxLengthUsername));

        RuleFor(x => x.Code).NotEmpty().WithMessage(AuthenticationValidationMessages.EmailConfirmCodeShouldNotBeEmpty)
                            .Must(x => x.Length == 6).WithMessage(AuthenticationValidationMessages.EmailConfirmCodeInvalid);
    }
}
