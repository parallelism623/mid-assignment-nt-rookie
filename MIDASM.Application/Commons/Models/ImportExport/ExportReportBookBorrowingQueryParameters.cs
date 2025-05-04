
namespace MIDASM.Application.Commons.Models.ImportExport;

public class ExportReportBookBorrowingQueryParameters
{
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public int Top { get; set; }
    public string ExportType { get; set; } = default!;
}
