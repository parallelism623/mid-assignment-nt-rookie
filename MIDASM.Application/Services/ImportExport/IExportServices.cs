using MIDASM.Application.Commons.Models.ImportExport;

namespace MIDASM.Application.Services.ImportExport;


public interface IExportServices
{
    ExportData<T> Export<T>(IEnumerable<T> dataExport);


}
