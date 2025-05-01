namespace Rookies.Contract.Exceptions;

public class BadRequestException : ExceptionBase
{
    public BadRequestException(string message) : base(message)
    {
    }
}