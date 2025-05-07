namespace MIDASM.Contract.Messages.ExceptionMessages;

public static class ApplicationExceptionMessages
{
    public static string NoSupportCryptoAlgorithmType => "No support {0} crypto algorithm";
    public static string CanNotCommitNullTransaction => "Transaction (in Transaction Manager) null, can not commit";
    public static string CanNotRollBackNullTransaction => "Transaction (in Transaction Manager) null, can not rollback";
    public static string CurrentTransactionNull = "Current transaction in database context is null";
    public static string TransactionWhenCommitNotMatchCurrentTransaction = "Current transaction does not match";
}
