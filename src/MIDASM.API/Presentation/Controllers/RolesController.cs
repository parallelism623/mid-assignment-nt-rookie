using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIDASM.Application.UseCases;

namespace MIDASM.API.Presentation.Controllers;

[Route("api/[controller]")]
public class RolesController(IRoleServices roleSerivces) : ApiBaseController
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAsync()
    { 
        var result = await roleSerivces.GetAsync();
        return ProcessResult(result);
    }
}
