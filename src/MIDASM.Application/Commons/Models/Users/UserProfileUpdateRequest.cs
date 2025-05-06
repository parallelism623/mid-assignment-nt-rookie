
using FluentValidation;
using MIDASM.Contract.Messages.Validations;
using MIDASM.Domain.Constrants.Validations;

namespace MIDASM.Application.Commons.Models.Users;

public class UserProfileUpdateRequest
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? PhoneNumber { get; set; }
}

public class UserProfileUpdateRequestValidator : AbstractValidator<UserProfileUpdateRequest>
{
    public UserProfileUpdateRequestValidator()
    {
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