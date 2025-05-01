using Microsoft.AspNetCore.Diagnostics;
using MIDASM.Contract.Exceptions;
using MIDASM.Contract.SharedKernel;
using Rookies.Contract.Exceptions;

namespace MIDASM.API.Middlewares;

public class ExceptionHandlerMiddleware : IExceptionHandler
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, exception.Message);
        var statusCode = GetExceptionResponseStatusCode(exception);
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var message = GetExceptionResponseMessage(exception) ?? "";
        var errorResponse = new Result(statusCode, false,  new Error(message, exception.Message));
        await context.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
        return true;
    }

    private static int GetExceptionResponseStatusCode(Exception exception)
    {
        return exception switch
        {
            BadRequestException => 400,
            NotFoundException => 404,
            ValidationException => 400,
            UnAuthorizedException => 401,
            InternalServerErrorException => 500,
            _ => 400
        };
    }

    private static string GetExceptionResponseMessage(Exception exception)
    {
        return exception switch
        {
            BadRequestException => "Bad request",
            NotFoundException => "Not found",
            ValidationException => "Invalid model",
            UnAuthorizedException => "UnAuthorized",
            InternalServerErrorException => "Internal server error",
            _ => "Bad request"
        };
    }
}
