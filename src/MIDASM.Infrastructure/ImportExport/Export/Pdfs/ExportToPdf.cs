
using MIDASM.Application.Commons.Models.ImportExport;
using MIDASM.Application.Services.ImportExport;
using MIDASM.Contract.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Net.Mime;

namespace MIDASM.Infrastructure.ImportExport.Export;

public class ExportToPdf : IExportServices
{
    private const string ContentType = "application/pdf";
    private const string FileName = "report_{Date}/pdf";

    public ExportResponse Export<T>(ExportRequest<T> exportRequest)
    {
        var docs = new ExportToPdfBasicDocs<T>(exportRequest);
        var exportResponse = new ExportResponse();
        exportResponse.ContentType = ContentType;
        exportResponse.FileName = StringHelper.ReplacePlaceholders(FileName, DateOnly.FromDateTime(DateTime.UtcNow).ToString());
        exportResponse.DataBytes = docs.GeneratePdf();

        return exportResponse;
    }

}
