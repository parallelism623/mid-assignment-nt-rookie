
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIDASM.Application.Commons.Models.BookReviews;
using MIDASM.Application.UseCases;

namespace MIDASM.API.Presentation.Controllers;

[Route("api/book-reviews")]
public class BookReviewsController(IBookReviewServices bookReviewServices) : ApiBaseController
{
    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateBookReviewRequest bookReviewCreateRequest)
    {
        var result = await bookReviewServices.CreateBookReviewAsync(bookReviewCreateRequest);

        return ProcessResult(result);
    }

    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetPaginationItemsAsync([FromQuery] BookReviewQueryParameters bookReviewQueryParameters)
    {
        var result = await bookReviewServices.GetAsync(bookReviewQueryParameters);

        return ProcessResult(result);
    }

}
