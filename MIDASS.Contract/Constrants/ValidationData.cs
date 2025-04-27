
namespace MIDASS.Contract.Constrants;

public static class ValidationData
{
    public const string EmailRegexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{2,}$";
    public const int MinLengthPassword = 8;
    public const int MaxLengthPassword = 32;
    public const int MaxLengthUsername = 100;
    public const int MaxLengthEmail = 100;
    public const int MaxLengthLastName = 100;
    public const int MaxLengthFirstName = 100;
    public const int MaxLengthPhoneNumber = 20;
}
