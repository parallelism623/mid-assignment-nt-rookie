namespace Rookies.Contract.Exceptions;

public class InternalServerErrorException : ExceptionBase
{
    public InternalServerErrorException(string message) : base(message)
    {
    }
}