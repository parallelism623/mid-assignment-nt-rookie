
namespace MIDASM.Application.Commons.Models.Report;

public class ReportQueryParameters
{
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; } 
    public int Top { get; set; }
}
