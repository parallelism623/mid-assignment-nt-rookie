using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIDASM.Application.Commons.Models.Categories;
using MIDASM.Application.UseCases;

namespace MIDASM.API.Presentation.Controllers;

[Route("api/[controller]")]
public class CategoriesController : ApiBaseController
{
    private readonly ICategoryServices _categoryServices;
    public CategoriesController(ICategoryServices categoryServices)
    {
        _categoryServices = categoryServices;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAsync([FromQuery] CategoriesQueryParameters queryParameters)
    {
        var result = await _categoryServices.GetCategoriesAsync(queryParameters);
        return ProcessResult(result);
    }

    [HttpGet("v2")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetNameAsync()
    {
        var result = await _categoryServices.GetCategoriesAsync();
        return ProcessResult(result);
    }
    [HttpGet]
    [Route("{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _categoryServices.GetCategoryByIdAsync(id);
        return ProcessResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAsync([FromBody] CategoryCreateRequest createRequest)
    {
        var result = await _categoryServices.CreateCategoryAsync(createRequest);
        return ProcessResult(result);
    }
    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAsync([FromBody] CategoryUpdateRequest updateRequest)
    {
        var result = await _categoryServices.UpdateCategoryAsync(updateRequest);

        return ProcessResult(result);
    }

    [HttpDelete]
    [Route("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _categoryServices.DeleteCategoryAsync(id);
        return ProcessResult(result);
    }
}
