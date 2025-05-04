
namespace MIDASM.Application.Commons.Models.ImportExport;

public class ExportRequest<T>
{
    public IEnumerable<T> DataExport { get; set; } = new List<T>();

    public string? Title { get; set; } = default;
    public DateOnly? FromDate { get; set; } = default;
    public DateOnly? ToDate { get; set; } = default;

}
