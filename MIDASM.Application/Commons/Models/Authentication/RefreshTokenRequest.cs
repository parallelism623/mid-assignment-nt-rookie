
using FluentValidation;
using MIDASM.Contract.Messages.Validations;

namespace MIDASM.Application.Commons.Models.Authentication;

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