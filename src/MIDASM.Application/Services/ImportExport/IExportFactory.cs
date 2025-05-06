
using MIDASM.Application.Commons.Models.ImportExport;

namespace MIDASM.Application.Services.ImportExport;

public interface IExportFactory
{
    ExportResponse ExportFile<T>(string fileType, ExportRequest<T> exportRequest);
}
