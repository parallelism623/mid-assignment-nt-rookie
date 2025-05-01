
using FluentValidation;
using MIDASM.Contract.Messages.Validations;
using MIDASM.Domain.Constrants;

namespace MIDASM.Application.Commons.Models.Authentication;

public class RefreshEmailConfirmTokenRequest
{
    public string Username { get; set; } = default!;
}


public class RefreshEmailConfirmTokenRequestValidator : AbstractValidator<RefreshEmailConfirmTokenRequest>
{
    public RefreshEmailConfirmTokenRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage(AuthenticationValidationMessages.UsernameShouldBeNotEmpty)
            .Matches(UserValidationRules.RegexPatternUsername)
            .WithMessage(string.Format(AuthenticationValidationMessages.UsernameShouldMatchesRegexPattern,
                                       UserValidationRules.MaxLengthUsername));

    }
}