using FluentValidation;
using MIDASS.Contract.Constrants;
using MIDASS.Contract.Messages.Validations;
using System.Text.RegularExpressions;

namespace MIDASS.Application.Commons.Models.Authentication;

public class LoginRequest
{
    public string? Username { get; set; } = default;
    public string? Email { get; set; } = default;
    public string Password { get; set; } = default!;
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    private readonly string _validationMessagePasswordLength =
        string.Format(AuthenticationValidationMessages.PasswordShouldBeInRange, ValidationData.MinLengthPassword,
            ValidationData.MaxLengthPassword);

    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().When(x => string.IsNullOrEmpty(x.Email))
            .WithMessage(AuthenticationValidationMessages.EmailOrUsernameShouldBeProvided);

        RuleFor(x => x.Email)
            .NotEmpty().When(x => string.IsNullOrEmpty(x.Username))
            .WithMessage(AuthenticationValidationMessages.EmailOrUsernameShouldBeProvided);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(AuthenticationValidationMessages.PasswordShouldNotBeEmpty);

        RuleFor(x => x.Password.Length)
            .GreaterThanOrEqualTo(ValidationData.MinLengthPassword).WithMessage(_validationMessagePasswordLength)
            .LessThanOrEqualTo(ValidationData.MaxLengthPassword).WithMessage(_validationMessagePasswordLength);

        RuleFor(x => x.Email)
            .Must(e => Regex.IsMatch(e!, ValidationData.EmailRegexPattern))
            .When(x => !string.IsNullOrEmpty(x.Email));
    }
}