
namespace MIDASM.Contract.Messages.Validations;

public static class AuthenticationValidationMessages
{
    public const string PasswordShouldNotBeEmpty = "Password should not be empty";
    public const string EmailOrUsernameShouldBeProvided = "Email or username should be provided";

    public const string PasswordShouldBeInRange = "Password length must be in range {0} - {1}";
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

    public const string EmailConfirmCodeShouldNotBeEmpty = "Email confirm code should not be empty";
    public const string EmailConfirmCodeInvalid = "Email confirm code invalid";

    public const string UsernameShouldMatchesRegexPattern = "Username must start with a letter, 1 - {0} chars, only letters, numbers and underscore.";
    public const string PasswordMustBeNotEmpty = "Password must be not empty";
    public const string PasswordMustMatcheRegexPattern = "Password must be {0}-{1} chars, include uppercase, lowercase, number and special character.";
}
