
using Rookies.Contract.Exceptions;

namespace MIDASM.Contract.Exceptions;

public class ValidationException : ExceptionBase
{
    public ValidationException(string message) : base(message)
    {
    }
}
