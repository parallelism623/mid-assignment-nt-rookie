using FluentValidation;
using MIDASS.Contract.Constrants;
using MIDASS.Contract.Messages.Validations;

namespace MIDASS.Application.Commons.Models.Authentication;

public class RegisterRequest
{
    
    public string Email { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string? PhoneNumber { get; set; } = default;
    public string? OAuthAccessToken = default;
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(AuthenticationValidationMessages.EmailShouldBeNotEmpty)
            .Matches(ValidationData.EmailRegexPattern)
            .WithMessage(AuthenticationValidationMessages.EmailInvalid)
            .Must(x => x.Length <= ValidationData.MaxLengthEmail)
            .WithMessage(string.Format(AuthenticationValidationMessages.EmailShouldBeLessThanOrEqualMaxLength,
                ValidationData.MaxLengthEmail));
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage(AuthenticationValidationMessages.UsernameShouldBeNotEmpty)
            .Must(x => x.Length <= ValidationData.MaxLengthUsername)
            .WithMessage(string.Format(AuthenticationValidationMessages.UsernameShouldBeLessThanOrEqualMaxLength,
                ValidationData.MaxLengthUsername));

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage(AuthenticationValidationMessages.LastNameShouldBeNotEmpty)
            .Must(x => x.Length <= ValidationData.MaxLengthLastName)
            .WithMessage(string.Format(AuthenticationValidationMessages.LastNameShouldBeLessThanOrEqualMaxLength,
                ValidationData.MaxLengthLastName));
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(AuthenticationValidationMessages.FirstNameShouldBeNotEmpty)
            .Must(x => x.Length <= ValidationData.MaxLengthFirstName)
            .WithMessage(string.Format(AuthenticationValidationMessages.FirstNameShouldBeLessThanOrEqualMaxLength,
                ValidationData.MaxLengthFirstName));

        RuleFor(x => x.PhoneNumber)
            .Must(x => x.Length <= ValidationData.MaxLengthPhoneNumber)
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage(string.Format(AuthenticationValidationMessages.PhoneNumberShouldBeLessThanOrEqualMaxLength,
                ValidationData.MaxLengthPhoneNumber));
    }
}