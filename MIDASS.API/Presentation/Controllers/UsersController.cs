

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIDASS.Application.Commons.Models;
using MIDASS.Application.Commons.Models.Users;
using MIDASS.Application.UseCases;

namespace MIDASS.API.Presentation.Controllers;

[Route("api/[controller]")]
public class UsersController : ApiBaseController
{
    private readonly IUserServices _userServices;
    public UsersController(IUserServices userServices)
    {
        _userServices = userServices;
    }

    [HttpPost]
    [Authorize(Roles = "User")]
    [Route("book-borrowing")]
    public async Task<IActionResult> CreateBookBorrowingRequestAsync([FromBody] BookBorrowingRequestCreate bookBorrowingRequest)
    {
        var result = await _userServices.CreateBookBorrowingRequestAsync(bookBorrowingRequest);

        return ProcessResult(result);
    }

    [HttpGet]
    [Route("{id:guid}/book-borrowing-requests")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetBookBorrowingRequestByIdAsync(Guid id, QueryParameters queryParameters)
    {
        var result = await _userServices.GetBookBorrowingRequestByIdAsync(id, queryParameters);

        return ProcessResult(result);
    }
    [HttpGet]
    [Route("{id:guid}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _userServices.GetByIdAsync(id);

        return ProcessResult(result);
    }
}
