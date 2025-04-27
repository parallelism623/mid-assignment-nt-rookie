namespace Rookies.Contract.Exceptions;

public class InvalidModelRequestException : ExceptionBase
{
    public InvalidModelRequestException(string message) : base(message)
    {
    }
}