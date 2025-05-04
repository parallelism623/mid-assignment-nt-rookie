
namespace MIDASM.Application.Commons.Models.ImportExport;

public class ExportReportCategoriesQueryParameters
{
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public int Top { get; set; }
    public string ExportType { get; set; } = default!;
}
