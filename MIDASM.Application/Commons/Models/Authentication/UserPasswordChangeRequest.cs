
using FluentValidation;
using MIDASM.Contract.Messages.Validations;
using MIDASM.Domain.Constrants.Validations;

namespace MIDASM.Application.Commons.Models.Authentication;
public class UserPasswordChangeRequest
{
    public string Password { get; set; } = default!;
    public string OldPassword { get; set; } = default!;
}

public class UserPasswordChangeRequestValidator : AbstractValidator<UserPasswordChangeRequest>
{
    public UserPasswordChangeRequestValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .WithMessage(AuthenticationValidationMessages.CurrentPasswordShouldNotBeEmpty);
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(AuthenticationValidationMessages.NewPasswordShouldNotBeEmpty)
            .Matches(UserValidationRules.RegexPatternPassword)
            .WithMessage(string.Format(AuthenticationValidationMessages.NewPasswordMustMatcheRegexPattern,
                                       UserValidationRules.MinLengthPassword, UserValidationRules.MaxLengthPassword));
    }
}