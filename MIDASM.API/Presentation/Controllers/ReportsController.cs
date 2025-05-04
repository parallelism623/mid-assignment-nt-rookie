
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIDASM.Application.Commons.Models.Report;
using MIDASM.Application.UseCases;

namespace MIDASM.API.Presentation.Controllers;

[Route("api/[controller]")]
public class ReportsController(IReportServices reportServices) : ApiBaseController
{
    [HttpGet]
    [Route("book-borrowings")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetBookBorrowingReport([FromQuery] BookBorrowingReportQueryParameters bookBorrowingReportRequest)
    {
        var result = await reportServices.GetBookBorrowingReportAsync(bookBorrowingReportRequest);

        return ProcessResult(result);
    }

    [HttpGet]
    [Route("categories")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetCategoryReport([FromQuery] CategoryReportQueryParameters categoryReportQueryParameters)
    {
        var result = await reportServices.GetCategoryReportAsync(categoryReportQueryParameters);

        return ProcessResult(result);
    }

    [HttpGet]
    [Route("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserReport([FromQuery] UserEngagementReportQueryParameters userReportQueryParameters)
    {
        var result = await reportServices.GetUserReportAsync(userReportQueryParameters);

        return ProcessResult(result);
    }
}

