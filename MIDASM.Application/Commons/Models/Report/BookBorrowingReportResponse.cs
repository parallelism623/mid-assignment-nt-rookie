
namespace MIDASM.Application.Commons.Models.Report;

public class BookBorrowingReportResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Author { get; set; } = default!;
    public string Category { get; set; } = default!;
    public int TotalBorrow { get; set; }
    public int Quantity { get; set; }
    public int Available { get; set; }
}
