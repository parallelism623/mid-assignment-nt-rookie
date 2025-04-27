using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIDASS.Application.Commons.Models.BookBorrowingRequests;
using MIDASS.Application.UseCases;

namespace MIDASS.API.Presentation.Controllers;

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
    public async Task<IActionResult> ChangeStatusAsync([FromBody]BookBorrowingStatusUpdateRequest statusUpdateRequest)
    {
        var result = await _bookBorrowingRequestServices.ChangeStatusAsync(statusUpdateRequest);

        return ProcessResult(result);
    }
}
