namespace Rookies.Contract.Exceptions;

public class UnAuthorizedException : ExceptionBase
{
    public UnAuthorizedException(string message) : base(message)
    {
    }
}