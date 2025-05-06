
namespace MIDASM.Application.Commons.Models.Report;

public class CategoryReportResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public int TotalBook { get; set; }
    public int TotalBorrowRequest { get; set; }
    public int QuantityBook { get; set; }
    public int AvailableBook { get; set; }
    public Guid? MostRequestedBookId { get; set; }
    public string? MostRequestedBook { get; set; }
    public int? RequestCount { get; set; }
}

public class BookOfCategoryReportResponse
{
    public Guid BookId { get; set; }
    public int Quantity { get; set; }
    public int Available { get; set; }
    public int TotalBorrow { get; set; }
    public string Name { get; set; } = default!;
    public Guid CategoryId { get; set; }
}
