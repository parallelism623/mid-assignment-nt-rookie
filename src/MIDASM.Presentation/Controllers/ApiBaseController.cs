using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MIDASM.API.Attributes;
using MIDASM.Domain.SharedKernel;
using Rookies.Contract.Exceptions;

namespace MIDASM.Presentation.Controllers;

[ValidationRequestModel]
[ApiController]
public abstract class ApiBaseController : ControllerBase
{
    private string[] SupportFileResultType =
    {
        "application/pdf",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    };
    protected IActionResult OnResultSuccess<T>(T result)
        where T : Result
        => result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            _ => StatusCode(result.StatusCode, result)
        };

    protected IActionResult OnResultFailure<T>(T result)
        where T : Result
        => result.StatusCode switch
        {
            StatusCodes.Status400BadRequest => BadRequest(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(result.StatusCode, result)
        };

    protected IActionResult ProcessResult<T>(T result)
        where T : Result
    {
        if (result == null)
        {
            throw new BadRequestException("Response api must be not null");
        }
        return result.IsSuccess ? OnResultSuccess(result) : OnResultFailure(result);
    }

    protected IActionResult ProcessFileResult(byte[] result, string contentType, string fileName = default!)
    {
        if (result == null)
        {
            throw new BadRequestException("Response api must be not null");
        }
        if (!SupportFileResultType.Contains(contentType))
        {
            throw new BadRequestException("Response file type invalid");
        }
        return File(result, contentType, fileName);
    }
}
