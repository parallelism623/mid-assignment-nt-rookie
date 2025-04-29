using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIDASS.Application.Commons.Models;
using MIDASS.Application.Commons.Models.BookBorrowingRequestDetails;
using MIDASS.Application.UseCases;

namespace MIDASS.API.Presentation.Controllers;


[Route("api/book-borrowing-request-details")]
public class BookBorrowingRequestDetailsController(IBookBorrowingRequestDetailServices bookBorrowingRequestDetailServices) 
    : ApiBaseController
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetsAsync([FromQuery] QueryParameters queryParameters)
    {
        var result = await bookBorrowingRequestDetailServices.GetsAsync(queryParameters);
        return ProcessResult(result);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    [Route("{id:guid}/due-date-extend")]
    public async Task<IActionResult> AdjustDueDateExtendAsync(Guid id, [FromBody] BookBorrowedExtendDueDateRequest bookBorrowedExtendDueDateRequest)
    {
        var result = await bookBorrowingRequestDetailServices.AdjustExtenDueDateAsync(id, bookBorrowedExtendDueDateRequest.Status);
        return ProcessResult(result);
    }
}
