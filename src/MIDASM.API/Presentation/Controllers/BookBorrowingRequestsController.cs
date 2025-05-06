using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIDASM.Application.Commons.Models.BookBorrowingRequests;
using MIDASM.Application.UseCases;

namespace MIDASM.API.Presentation.Controllers;

[Route("api/book-borrowing-requests")]
public class BookBorrowingRequestsController : ApiBaseController
{
    private readonly IBookBorrowingRequestServices _bookBorrowingRequestServices;

    public BookBorrowingRequestsController(IBookBorrowingRequestServices bookBorrowingRequestServices)
    {
        _bookBorrowingRequestServices = bookBorrowingRequestServices;
    }
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetsAsync([FromQuery] BookBorrowingRequestQueryParameters queryParameters)
    {
        var result = await _bookBorrowingRequestServices.GetsAsync(queryParameters);

        return ProcessResult(result);
    }
    [HttpPut]
    [Route("status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatusAsync([FromBody]BookBorrowingStatusUpdateRequest statusUpdateRequest)
    {
        var result = await _bookBorrowingRequestServices.ChangeStatusAsync(statusUpdateRequest);

        return ProcessResult(result);
    }
    [HttpGet]
    [Route("{id:guid}/detail")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> GetDetailByIdAsync(Guid id)
    {
        var result = await _bookBorrowingRequestServices.GetDetailAsync(id);
        return ProcessResult(result);
    }
}
