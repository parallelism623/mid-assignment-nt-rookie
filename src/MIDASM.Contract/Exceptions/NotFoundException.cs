namespace Rookies.Contract.Exceptions;

public class NotFoundException : ExceptionBase
{
    public NotFoundException(string message) : base(message)
    {
    }
}