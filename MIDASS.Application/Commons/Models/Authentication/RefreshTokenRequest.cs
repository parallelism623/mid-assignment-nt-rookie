
using FluentValidation;
using MIDASS.Contract.Messages.Validations;

namespace MIDASS.Application.Commons.Models.Authentication;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
}


public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage(AuthenticationValidationMessages.RefreshTokenShouldNotBeEmpty);
        RuleFor(x => x.AccessToken)
            .NotEmpty()
            .WithMessage(AuthenticationValidationMessages.AccessTokenShouldNotBeEmpty);
    }
}