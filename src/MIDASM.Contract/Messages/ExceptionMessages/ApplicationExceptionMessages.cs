namespace MIDASM.Contract.Messages.ExceptionMessages;

public static class ApplicationExceptionMessages
{
    public static string NoSupportCryptoAlgorithmType => "No support {0} crypto algorithm";
    public static string CanNotCommitNullTransaction => "Transaction (in Transaction Manager) null, can not commit";
    public static string CanNotRollBackNullTransaction => "Transaction (in Transaction Manager) null, can not rollback";
    public static string CurrentTransactionNull => "Current transaction in database context is null";
    public static string TransactionWhenCommitNotMatchCurrentTransaction => "Current transaction does not match";
    public static string UserNameInvalid => "User invalid";
    public static string EmailConfirmCodeInvalid => "Email confirm code invalid";

    public static string EmailOrUsernameIncorrect => "Username or email incorrect";
    public static string PasswordIncorrect => "Password incorrect";

    public static string UsernameAlreadyExists => "Username already exists";
    public static string EmailAlreadyExists => "Email already exists";

    public static string RoleUserDoesNotExists => "Role User does not exists";

    public static string UserOrEmailMustBeProvided => "Username or email should be provided";

    public static string UserNull => "User null";
    public static string InvalidRefreshToken => "Invalid refresh token";

    public static string UserIdInExecutionContextInvalid => "User id in execution context invalid";
    public static string SignatureAlgorithmJwtTokenInvalid => "Invalid signature algorithm jwt token";
    public static string InvalidAccessToken => "Invalid access token";
}
