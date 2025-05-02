

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIDASM.Application.Commons.Models;
using MIDASM.Application.Commons.Models.Users;
using MIDASM.Application.UseCases;

namespace MIDASM.API.Presentation.Controllers;

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
    public async Task<IActionResult> GetBookBorrowingRequestByIdAsync(Guid id, [FromQuery] UserBookBorrowingRequestQueryParameters queryParameters)
    {
        var result = await _userServices.GetBookBorrowingRequestByIdAsync(id, queryParameters);

        return ProcessResult(result);
    }
    [HttpGet]
    [Route("{id:guid}/book-borrowed")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetBookBorrowedRequestDetailByIdAsync(Guid id, [FromQuery] QueryParameters queryParameters)
    {
        var result = await _userServices.GetBookBorrowedRequestDetailByIdAsync(id, queryParameters);

        return ProcessResult(result);
    }
    [HttpPut]
    [Route("{id:guid}/book-borrowed/due-date-extend")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> ExtendDueDateAsync (Guid id, [FromBody] DueDatedExtendRequest dueDatedExtendRequest)
    {
        var result = await _userServices.ExtendDueDateBookBorrowed(dueDatedExtendRequest);
            
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
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAsync([FromQuery] UserQueryParameters queryParameters)
    {
        var result = await _userServices.GetAsync(queryParameters);
        return ProcessResult(result);
    }
}
