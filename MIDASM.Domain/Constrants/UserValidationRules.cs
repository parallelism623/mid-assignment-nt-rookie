
namespace MIDASM.Domain.Constrants;

public static class UserValidationRules
{
    public const int MaxLengthEmail = 100;
    public const int MaxLengthFirstName = 100;
    public const int MaxLengthLastName = 100;
    public const int MaxLengthHashPassword = 500;
    public const int MaxLengthPassword = 32;
    public const int MinLengthPassword = 8;
    public const int MaxLengthUsername = 32;
    public const int MaxLengthPhoneNumber = 20;

    public const string RegexPatternEmail = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
    public const string RegexPatternUsername = @"^[A-Za-z0-9_]{3,32}$";
    public const string RegexPatternPassword = @"^(?=.{8,32}$)(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$";
}
