
namespace MIDASM.Application.Commons.Models.ImportExport;

public class ExportData<T>
{
    public string ContentType { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public IEnumerable<T> Data { get; set; } = default!;

    public byte[] DataBytes { get; set; } = default!;

}
