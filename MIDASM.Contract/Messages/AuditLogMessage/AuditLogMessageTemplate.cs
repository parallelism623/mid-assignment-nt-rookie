
namespace MIDASM.Contract.Messages.AuditLogMessage;

public static class AuditLogMessageTemplate
{
    public static string Create => @"{Username} created {Entity} {EntityName} on {TimeStamp}";
    public static string Update => @"{Username} updated {Entity} {EntityName} on {TimeStamp} || {ChangesDescription}";
    public static string Delete => @"{Username} deleted {Entity} {EntityName} on {TimeStamp}";

    public static string AdjustDueDateExtend => @"{Username} {Status} due date extend book {BookName} of {UserRequset} on {TimeStamp}";
    public static string UpdateBookBorrowingRequestStatus => @"{Username} {Status} book borrowing request of {UserRequest} on {TimeStamp}";
    public static string CreateBookBorrowingRequest => @"{Username} created book borrowing {RequestId} request on {TimeStamp}";
    public static string ExtendDueDate => @"{Username} created extend due date for {Book} from {OldDate} to {NewDate} on {TimeStamp}";

    public static string UserLogin => @"{Username} was logged on  at {TimeStamp} || {UserAgent}";
    public static string UserLogout => @"{Username} was logged out at {TimeStamp} || {UserAgent}";
    public static string UserRegister => @"New {Username} was register at {TimeStamp}";
}



