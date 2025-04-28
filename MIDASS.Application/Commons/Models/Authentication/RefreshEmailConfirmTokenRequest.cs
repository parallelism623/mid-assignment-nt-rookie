
using FluentValidation;
using MIDASS.Contract.Messages.Commands;
using MIDASS.Contract.Messages.Validations;

namespace MIDASS.Application.Commons.Models.Authentication;

public class RefreshEmailConfirmTokenRequest
{
    public string Username { get; set; } = default!;
}


public class RefreshEmailConfirmTokenRequestValidator : AbstractValidator<RefreshEmailConfirmTokenRequest>
{
    public RefreshEmailConfirmTokenRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage(AuthenticationValidationMessages.UsernameShouldBeNotEmpty);
    }
}