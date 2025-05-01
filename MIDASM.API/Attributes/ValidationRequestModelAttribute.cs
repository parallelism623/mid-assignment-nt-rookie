using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MIDASM.Contract.SharedKernel;

namespace MIDASM.API.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class ValidationRequestModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .Select(x => new Error(x.Key, x.Value?.Errors?.FirstOrDefault()?.ErrorMessage))
                .ToList();
            var errorResponse = new Result(400, false);
            if (errors.Any())
            {
                errorResponse.Errors = errors;
            }
            context.Result = new JsonResult(errorResponse)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
    }
}
