
using Rookies.Contract.Exceptions;

namespace MIDASS.Contract.Exceptions;

public class ValidationException : ExceptionBase
{
    public ValidationException(string message) : base(message)
    {
    }
}
