
using FluentValidation;
using MIDASS.Contract.Messages.Validations;
using MIDASS.Domain.Constrants;

namespace MIDASS.Application.Commons.Models.Authentication;

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