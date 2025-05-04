using Microsoft.AspNetCore.Mvc;
using MIDASM.API.Attributes;
using MIDASM.Contract.SharedKernel;

namespace MIDASM.API.Presentation.Controllers;

[ValidationRequestModel]
[ApiController]
public abstract class ApiBaseController : ControllerBase
{
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
        => result.IsSuccess ? OnResultSuccess(result) : OnResultFailure(result);

    protected IActionResult ProcessFileResult(byte[] result, string contentType, string fileName)
    {
        return File(result, contentType, fileName);
    }
}
