
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIDASM.Application.Commons.Models.ImportExport;
using MIDASM.Application.Commons.Models.Report;
using MIDASM.Application.Services.ImportExport;
using MIDASM.Application.UseCases;
using MIDASM.Contract.Helpers;
using MIDASM.Contract.Messages.ExportTemplateMessages;

namespace MIDASM.API.Presentation.Controllers;

[Route("api/[controller]")]
public class ExportsController(IExportFactory exportFactory, IReportServices reportServices): ApiBaseController
{
    [HttpGet]
    [Route("reports/users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetExportFileReportUserEngagementAsync([FromQuery] ExportReportUserEngagementQueryParameters exportQueryParameter)
    {
        var dataReport = await reportServices.GetUserReportAsync(new UserEngagementReportQueryParameters {
            ToDate = exportQueryParameter.ToDate,
            FromDate = exportQueryParameter.FromDate,
            Top = exportQueryParameter.Top
        }); 
        var exportRequest = new ExportRequest<UserReportResponse>
        {
            DataExport = dataReport?.Data?.Items!,
            Title = StringHelper.ReplacePlaceholders(ExportTemplateMessage.TilteExportFile, exportQueryParameter.Top.ToString(), "User Engagement"),
            ToDate = exportQueryParameter.ToDate,
            FromDate = exportQueryParameter.FromDate,
        };
        var result = exportFactory.ExportFile(exportQueryParameter.ExportType, exportRequest);

        return ProcessFileResult(result.DataBytes, result.ContentType, result.FileName);
    }
    [HttpGet]
    [Route("reports/categories")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetExportFileReportCategoriesAsync([FromQuery] ExportReportCategoriesQueryParameters exportQueryParameter)
    {
        var dataReport = await reportServices.GetCategoryReportAsync(new CategoryReportQueryParameters
        {
            ToDate = exportQueryParameter.ToDate,
            FromDate = exportQueryParameter.FromDate,
            Top = exportQueryParameter.Top
        });
        var exportRequest = new ExportRequest<CategoryReportResponse>
        {
            DataExport = dataReport?.Data?.Items!,
            Title = StringHelper.ReplacePlaceholders(ExportTemplateMessage.TilteExportFile, exportQueryParameter.Top.ToString(), "Categories"),
            ToDate = exportQueryParameter.ToDate,
            FromDate = exportQueryParameter.FromDate,
        };
        var result = exportFactory.ExportFile(exportQueryParameter.ExportType, exportRequest);

        return ProcessFileResult(result.DataBytes, result.ContentType, result.FileName);
    }
    [HttpGet]
    [Route("reports/book-borrowings")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetExportFileReportBookBorrowingAsync([FromQuery] ExportReportBookBorrowingQueryParameters exportQueryParameter)
    {
        var dataReport = await reportServices.GetBookBorrowingReportAsync(new BookBorrowingReportQueryParameters 
        {
            ToDate = exportQueryParameter.ToDate,
            FromDate = exportQueryParameter.FromDate,
            Top = exportQueryParameter.Top
        });

        var exportRequest = new ExportRequest<BookBorrowingReportResponse>
        {
            DataExport = dataReport?.Data?.Items!,
            Title = StringHelper.ReplacePlaceholders(ExportTemplateMessage.TilteExportFile, exportQueryParameter.Top.ToString(), "Book Borrowing"),
            ToDate = exportQueryParameter.ToDate,
            FromDate = exportQueryParameter.FromDate,
        };
        var result = exportFactory.ExportFile(exportQueryParameter.ExportType, exportRequest);

        return ProcessFileResult(result.DataBytes, result.ContentType, result.FileName);
    }
}
