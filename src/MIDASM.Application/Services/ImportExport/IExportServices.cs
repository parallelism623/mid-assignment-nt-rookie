using MIDASM.Application.Commons.Models.ImportExport;

namespace MIDASM.Application.Services.ImportExport;


public interface IExportServices
{
    ExportResponse Export<T>(ExportRequest<T> exportRequest);

}
