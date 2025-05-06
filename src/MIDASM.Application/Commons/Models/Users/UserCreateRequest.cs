

using FluentValidation;
using MIDASM.Contract.Messages.Validations;
using MIDASM.Domain.Constrants.Validations;

namespace MIDASM.Application.Commons.Models.Users;

public class UserCreateRequest
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? PhoneNumber { get; set; } = default!;
    public Guid RoleId { get; set; }    
}

public class UserCreateRequestValidator : AbstractValidator<UserCreateRequest>
{
    public UserCreateRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage(UserValidationMessages.UserRoleMustNotEmpty);
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(AuthenticationValidationMessages.PasswordMustBeNotEmpty)
            .Matches(UserValidationRules.RegexPatternPassword)
            .WithMessage(string.Format(AuthenticationValidationMessages.PasswordMustMatcheRegexPattern,
                                       UserValidationRules.MinLengthPassword, UserValidationRules.MaxLengthPassword));
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(AuthenticationValidationMessages.EmailShouldBeNotEmpty)
            .Matches(UserValidationRules.RegexPatternEmail)
            .WithMessage(AuthenticationValidationMessages.EmailInvalid)
            .Must(x => x.Length <= UserValidationRules.MaxLengthEmail)
            .WithMessage(string.Format(AuthenticationValidationMessages.EmailShouldBeLessThanOrEqualMaxLength,
                UserValidationRules.MaxLengthEmail));
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage(AuthenticationValidationMessages.UsernameShouldBeNotEmpty)
            .Matches(UserValidationRules.RegexPatternUsername)
            .WithMessage(string.Format(AuthenticationValidationMessages.UsernameShouldMatchesRegexPattern,
                                       UserValidationRules.MaxLengthUsername));
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage(AuthenticationValidationMessages.LastNameShouldBeNotEmpty)
            .Must(x => x.Length <= UserValidationRules.MaxLengthLastName)
            .WithMessage(string.Format(AuthenticationValidationMessages.LastNameShouldBeLessThanOrEqualMaxLength,
                UserValidationRules.MaxLengthLastName));
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(AuthenticationValidationMessages.FirstNameShouldBeNotEmpty)
            .Must(x => x.Length <= UserValidationRules.MaxLengthFirstName)
            .WithMessage(string.Format(AuthenticationValidationMessages.FirstNameShouldBeLessThanOrEqualMaxLength,
                UserValidationRules.MaxLengthFirstName));

        RuleFor(x => x.PhoneNumber)
            .Must(x => x == null || x.Length <= UserValidationRules.MaxLengthPhoneNumber)
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage(string.Format(AuthenticationValidationMessages.PhoneNumberShouldBeLessThanOrEqualMaxLength,
                UserValidationRules.MaxLengthPhoneNumber));

    }
}
