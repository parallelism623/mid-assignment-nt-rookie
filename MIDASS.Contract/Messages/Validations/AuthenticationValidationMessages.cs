
namespace MIDASS.Contract.Messages.Validations;

public static class AuthenticationValidationMessages
{
    public const string PasswordShouldNotBeEmpty = "Password should not be empty";
    public const string EmailOrUsernameShouldBeProvided = "Email or username should be provided";

    public const string PasswordShouldBeInRange = "Password length should be in rang {0} - {1}";
    public const string EmailInvalid = "Email invalid";
    public const string EmailShouldBeNotEmpty = "Email should be not empty";
    public const string UsernameShouldBeNotEmpty = "Username should be not empty";
    public const string UsernameShouldBeLessThanOrEqualMaxLength = "Username should be less than or equal {0}";
    public const string EmailShouldBeLessThanOrEqualMaxLength = "Email should be less than or equal {0}";

    public const string FirstNameShouldBeNotEmpty = "First name should be not empty";
    public const string LastNameShouldBeNotEmpty = "Last name should be not empty";
    public const string FirstNameShouldBeLessThanOrEqualMaxLength = "First name should be less than or equal {0}";
    public const string LastNameShouldBeLessThanOrEqualMaxLength = "Last name should be less than or equal {0}";
    public const string PhoneNumberShouldBeLessThanOrEqualMaxLength = "Phone number should be less than or equal {0}";
    public const string RefreshTokenShouldNotBeEmpty = "Refresh token should not be empty";
    public const string AccessTokenShouldNotBeEmpty = "Access token should not be empty";
}
