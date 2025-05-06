
namespace MIDASM.Application.Commons.Models.ImportExport;

public class ExportResponse
{
    public string ContentType { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public byte[] DataBytes { get; set; } = default!;

}
