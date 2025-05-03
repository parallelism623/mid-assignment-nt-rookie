using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIDASM.Application.Commons.Models.Books;
using MIDASM.Application.UseCases;

namespace MIDASM.API.Presentation.Controllers;

[Route("api/[controller]")]
public class BooksController(IBookServices bookServices)
    : ApiBaseController
{
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetsAsync([FromQuery] BooksQueryParameters queryParameters)
    {
        var result = await bookServices.GetsAsync(queryParameters);

        return ProcessResult(result);
    }
    
    [HttpGet]
    [Route("{id:guid}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await bookServices.GetByIdAsync(id);

        return ProcessResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAsync([FromForm] BookCreateRequest request)
    {
        var result = await bookServices.CreateAsync(request);

        return ProcessResult(result);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAsync([FromForm] BookUpdateRequest request)
    {
        var result = await bookServices.UpdateAsync(request);

        return ProcessResult(result);
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    [Route("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await bookServices.DeleteAsync(id);

        return ProcessResult(result);
    }

}
