
using FluentValidation;
using MIDASM.Contract.Messages.Validations;
using MIDASM.Domain.Constrants;

namespace MIDASM.Application.Commons.Models.Users;

public class UserUpdateRequest
{
    public Guid Id { get; set; }
    public string? Password { get; set; } = default;
    public string Email { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }  = string.Empty;

    public Guid RoleId { get; set; }

}

public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequest>
{
    public UserUpdateRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage(UserValidationMessages.UserRoleMustNotEmpty);
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(UserValidationMessages.UserIdMustBeNotEmpty);
        RuleFor(x => x.Password)
            .Matches(UserValidationRules.RegexPatternPassword)
            .When(x => !string.IsNullOrEmpty(x.Password))
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